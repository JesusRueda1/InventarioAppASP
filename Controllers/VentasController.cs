// ============================================================
// Controllers/VentasController.cs  –  POS de ventas
// ============================================================
using InventarioApp.Data;
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
    public async Task<IActionResult> Index()
    {
        ViewBag.Productos = await _db.Productos.Where(p => p.Stock > 0).ToListAsync();
        return View();
    }

    // GET /Ventas/ObtenerTodas
    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var lista = await _db.Ventas
            .OrderByDescending(v => v.Fecha)
            .Select(v => new { v.Id, Fecha = v.Fecha.ToString("dd/MM/yyyy HH:mm"), v.Total })
            .ToListAsync();
        return Json(lista);
    }

    // GET /Ventas/ObtenerDetalle/5
    [HttpGet]
    public async Task<IActionResult> ObtenerDetalle(int id)
    {
        var detalle = await _db.DetalleVentas
            .Where(d => d.VentaId == id)
            .Include(d => d.Producto)
            .Select(d => new
            {
                ProductoNombre = d.Producto!.Nombre,
                d.Cantidad,
                d.PrecioVenta,
                Subtotal = d.Cantidad * d.PrecioVenta
            }).ToListAsync();
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
    public async Task<IActionResult> Registrar([FromBody] VentaDto dto)
    {
        if (dto.Detalles == null || !dto.Detalles.Any())
            return BadRequest(new { mensaje = "Agrega al menos un producto." });

        // Validar stock disponible antes de guardar
        foreach (var item in dto.Detalles)
        {
            var producto = await _db.Productos.FindAsync(item.ProductoId);
            if (producto == null)
                return BadRequest(new { mensaje = $"Producto {item.ProductoId} no encontrado." });
            if (producto.Stock < item.Cantidad)
                return BadRequest(new { mensaje = $"Stock insuficiente para '{producto.Nombre}'." });
        }

        var venta = new Venta
        {
            Fecha = DateTime.Now,
            Total = dto.Detalles.Sum(d => d.Cantidad * d.PrecioVenta)
        };
        _db.Ventas.Add(venta);
        await _db.SaveChangesAsync();

        foreach (var item in dto.Detalles)
        {
            _db.DetalleVentas.Add(new DetalleVenta
            {
                VentaId     = venta.Id,
                ProductoId  = item.ProductoId,
                Cantidad    = item.Cantidad,
                PrecioVenta = item.PrecioVenta
            });

            var producto = await _db.Productos.FindAsync(item.ProductoId);
            if (producto != null) producto.Stock -= item.Cantidad;
        }

        await _db.SaveChangesAsync();
        return Ok(new { mensaje = "Venta registrada", id = venta.Id, total = venta.Total });
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
