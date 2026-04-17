// ============================================================
// Controllers/ComprasController.cs
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
public class ComprasController : Controller
{
    private readonly ApplicationDbContext _db;
    public ComprasController(ApplicationDbContext db) => _db = db;

    // GET /Compras
    [RequirePermission("compras.ver")]
    public async Task<IActionResult> Index()
    {
        ViewBag.Productos = await _db.Productos.ToListAsync();
        return View();
    }

    // GET /Compras/ObtenerTodas
    [HttpGet]
    [RequirePermission("compras.ver")]
    public async Task<IActionResult> ObtenerTodas()
    {
        var lista = await _db.Transacciones
            .Where(t => t.Tipo == TipoTransaccion.Compra)
            .OrderByDescending(t => t.Fecha)
            .Select(t => new
            {
                t.Id,
                Fecha     = t.Fecha.ToString("dd/MM/yyyy HH:mm"),
                t.Proveedor,
                t.Total
            })
            .ToListAsync();

        return Json(lista);
    }

    // GET /Compras/ObtenerDetalle/5
    [HttpGet]
    [RequirePermission("compras.ver")]
    public async Task<IActionResult> ObtenerDetalle(int id)
    {
        var detalle = await _db.DetalleCompras
            .Where(d => d.TransaccionId == id)
            .Include(d => d.Producto)
            .Select(d => new
            {
                ProductoNombre = d.Producto!.Nombre,
                d.Cantidad,
                d.PrecioCosto,
                Subtotal = d.Cantidad * d.PrecioCosto
            })
            .ToListAsync();

        return Json(detalle);
    }

    // POST /Compras/Registrar
    [HttpPost]
    [RequirePermission("compras.registrar")]
    public async Task<IActionResult> Registrar([FromBody] CompraDto dto)
    {
        if (dto.Detalles == null || !dto.Detalles.Any())
            return BadRequest(new { mensaje = "Agrega al menos un producto." });

        decimal subtotalGlobal = 0;
        decimal impuestoGlobal = 0;
        var detallesAGuardar = new List<DetalleCompra>();
        var kardexLogs = new List<MovimientoKardex>();

        foreach (var item in dto.Detalles)
        {
            var producto = await _db.Productos.Include(p => p.Impuesto).FirstOrDefaultAsync(p => p.Id == item.ProductoId);
            if (producto == null)
                return BadRequest(new { mensaje = $"Producto {item.ProductoId} no encontrado." });

            decimal subtotalLinea = item.Cantidad * item.PrecioCosto;
            decimal porcImpuesto = producto.Impuesto?.Porcentaje ?? 0;
            decimal montoImpuesto = subtotalLinea * (porcImpuesto / 100);

            subtotalGlobal += subtotalLinea;
            impuestoGlobal += montoImpuesto;

            // Sumar inventario físicamente
            producto.Stock += item.Cantidad;

            detallesAGuardar.Add(new DetalleCompra
            {
                ProductoId = producto.Id,
                Cantidad = item.Cantidad,
                PrecioCosto = item.PrecioCosto,
                PorcentajeImpuesto = porcImpuesto,
                MontoImpuesto = montoImpuesto
            });

            kardexLogs.Add(new MovimientoKardex
            {
                ProductoId = producto.Id,
                Fecha = DateTime.Now,
                Tipo = TipoMovimientoKardex.Ingreso,
                Cantidad = item.Cantidad,
                Saldo = producto.Stock,
                Motivo = $"Entrada por compra a proveedor {(dto.Proveedor ?? "Global")}",
                UsuarioId = User.UserId()
            });
        }

        var transaccion = new Transaccion
        {
            Tipo      = TipoTransaccion.Compra,
            Fecha     = DateTime.Now,
            Proveedor = dto.Proveedor,
            Subtotal  = subtotalGlobal,
            TotalImpuesto = impuestoGlobal,
            Total     = subtotalGlobal + impuestoGlobal,
            EstadoPago = EstadoPagoTransaccion.Pendiente, // Por defecto compras entran en cartera
            SaldoPendiente = subtotalGlobal + impuestoGlobal,
            UsuarioId = User.UserId()
        };

        _db.Transacciones.Add(transaccion);
        await _db.SaveChangesAsync();

        foreach (var d in detallesAGuardar)
        {
            d.TransaccionId = transaccion.Id;
            _db.DetalleCompras.Add(d);
        }

        foreach (var k in kardexLogs)
        {
            k.TransaccionId = transaccion.Id;
            _db.MovimientosKardex.Add(k);
        }

        await _db.SaveChangesAsync();
        return Ok(new { mensaje = "Compra registrada", id = transaccion.Id, total = transaccion.Total });
    }
}

// ── DTOs ─────────────────────────────────────────────────────
public class CompraDto
{
    public string? Proveedor { get; set; }
    public List<DetalleCompraDto> Detalles { get; set; } = new();
}

public class DetalleCompraDto
{
    public int     ProductoId  { get; set; }
    public int     Cantidad    { get; set; }
    public decimal PrecioCosto { get; set; }
}
