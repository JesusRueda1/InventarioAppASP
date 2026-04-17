// ============================================================
// Controllers/VentasController.cs  –  POS de ventas
// ============================================================
using InventarioApp.Authorization;
using InventarioApp.Data;
using InventarioApp.Extensions;
using InventarioApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioApp.Controllers;

[Authorize]
public class VentasController : Controller
{
    private readonly ApplicationDbContext _db;
    public VentasController(ApplicationDbContext db) => _db = db;

    // GET /Ventas
    [RequirePermission("ventas.ver")]
    public async Task<IActionResult> Index()
    {
        ViewBag.Productos = await _db.Productos.Where(p => p.Stock > 0).ToListAsync();
        return View();
    }

    // GET /Ventas/ObtenerTodas
    [HttpGet]
    [RequirePermission("ventas.ver")]
    public async Task<IActionResult> ObtenerTodas()
    {
        var lista = await _db.Transacciones
            .Where(t => t.Tipo == TipoTransaccion.Venta)
            .OrderByDescending(t => t.Fecha)
            .Select(t => new
            {
                t.Id,
                Fecha = t.Fecha.ToString("dd/MM/yyyy HH:mm"),
                t.Total
            })
            .ToListAsync();

        return Json(lista);
    }

    // GET /Ventas/ObtenerDetalle/5
    [HttpGet]
    [RequirePermission("ventas.ver")]
    public async Task<IActionResult> ObtenerDetalle(int id)
    {
        var detalle = await _db.DetalleVentas
            .Where(d => d.TransaccionId == id)
            .Include(d => d.Producto)
            .Select(d => new
            {
                ProductoNombre = d.Producto!.Nombre,
                d.Cantidad,
                d.PrecioVenta,
                Subtotal = d.Cantidad * d.PrecioVenta
            })
            .ToListAsync();

        return Json(detalle);
    }

    // GET /Ventas/BuscarProducto?q=lapt
    [HttpGet]
    public async Task<IActionResult> BuscarProducto(string q)
    {
        var productos = await _db.Productos
            .Where(p => p.Stock > 0 && p.Nombre.Contains(q))
            .Select(p => new { p.Id, p.Nombre, p.Precio, p.Stock })
            .Take(10)
            .ToListAsync();

        return Json(productos);
    }

    // POST /Ventas/Registrar
    [HttpPost]
    [RequirePermission("ventas.registrar")]
    public async Task<IActionResult> Registrar([FromBody] VentaDto dto)
    {
        if (dto.Detalles == null || !dto.Detalles.Any())
            return BadRequest(new { mensaje = "Agrega al menos un producto." });

        decimal subtotalGlobal = 0;
        decimal impuestoGlobal = 0;
        var detallesAGuardar = new List<DetalleVenta>();
        var kardexLogs = new List<MovimientoKardex>();

        // Validar stock disponible y calcular impuestos línea por línea
        foreach (var item in dto.Detalles)
        {
            var producto = await _db.Productos.Include(p => p.Impuesto).FirstOrDefaultAsync(p => p.Id == item.ProductoId);
            if (producto == null)
                return BadRequest(new { mensaje = $"Producto {item.ProductoId} no encontrado." });
            if (producto.Stock < item.Cantidad)
                return BadRequest(new { mensaje = $"Stock insuficiente para '{producto.Nombre}'." });

            decimal subtotalLinea = item.Cantidad * item.PrecioVenta;
            decimal porcImpuesto = producto.Impuesto?.Porcentaje ?? 0;
            decimal montoImpuesto = subtotalLinea * (porcImpuesto / 100);

            subtotalGlobal += subtotalLinea;
            impuestoGlobal += montoImpuesto;

            // Restar inventario físicamente
            producto.Stock -= item.Cantidad;

            detallesAGuardar.Add(new DetalleVenta
            {
                ProductoId = producto.Id,
                Cantidad = item.Cantidad,
                PrecioVenta = item.PrecioVenta,
                PorcentajeImpuesto = porcImpuesto,
                MontoImpuesto = montoImpuesto
            });

            kardexLogs.Add(new MovimientoKardex
            {
                ProductoId = producto.Id,
                Fecha = DateTime.Now,
                Tipo = TipoMovimientoKardex.Egreso,
                Cantidad = item.Cantidad,
                Saldo = producto.Stock,
                Motivo = "Venta facturada en terminal POS",
                UsuarioId = User.UserId()
            });
        }

        var transaccion = new Transaccion
        {
            Tipo      = TipoTransaccion.Venta,
            Fecha     = DateTime.Now,
            Subtotal  = subtotalGlobal,
            TotalImpuesto = impuestoGlobal,
            Total     = subtotalGlobal + impuestoGlobal,
            EstadoPago = EstadoPagoTransaccion.Pagado, // Por el momento POS asume pago inmediato
            SaldoPendiente = 0,
            UsuarioId = User.UserId()
        };

        _db.Transacciones.Add(transaccion);
        await _db.SaveChangesAsync();

        foreach (var d in detallesAGuardar)
        {
            d.TransaccionId = transaccion.Id;
            _db.DetalleVentas.Add(d);
        }

        foreach (var k in kardexLogs)
        {
            k.TransaccionId = transaccion.Id;
            _db.MovimientosKardex.Add(k);
        }

        await _db.SaveChangesAsync();
        return Ok(new { mensaje = "Venta registrada", id = transaccion.Id, total = transaccion.Total });
    }
    // GET /Ventas/Ticket/5
    [HttpGet]
    [AllowAnonymous] // Dependiendo de las necesidades de seguridad
    public async Task<IActionResult> Ticket(int id)
    {
        var transaccion = await _db.Transacciones
            .Include(t => t.DetallesVenta)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaccion == null) return NotFound();
        return View(transaccion);
    }

    // GET /Ventas/Factura/5
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Factura(int id)
    {
        var transaccion = await _db.Transacciones
            .Include(t => t.DetallesVenta)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaccion == null) return NotFound();
        return View(transaccion);
    }
}

// ── DTOs ─────────────────────────────────────────────────────
public class VentaDto
{
    public List<DetalleVentaDto> Detalles { get; set; } = new();
}

public class DetalleVentaDto
{
    public int     ProductoId  { get; set; }
    public int     Cantidad    { get; set; }
    public decimal PrecioVenta { get; set; }
}
