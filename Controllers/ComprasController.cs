// ============================================================
// Controllers/ComprasController.cs
// ============================================================
using InventarioApp.Data;
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
    public async Task<IActionResult> Index()
    {
        ViewBag.Productos = await _db.Productos.ToListAsync();
        return View();
    }

    // GET /Compras/ObtenerTodas
    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var lista = await _db.Compras
            .OrderByDescending(c => c.Fecha)
            .Select(c => new { c.Id, Fecha = c.Fecha.ToString("dd/MM/yyyy HH:mm"), c.Proveedor, c.Total })
            .ToListAsync();
        return Json(lista);
    }

    // GET /Compras/ObtenerDetalle/5
    [HttpGet]
    public async Task<IActionResult> ObtenerDetalle(int id)
    {
        var detalle = await _db.DetalleCompras
            .Where(d => d.CompraId == id)
            .Include(d => d.Producto)
            .Select(d => new
            {
                ProductoNombre = d.Producto!.Nombre,
                d.Cantidad,
                d.PrecioCosto,
                Subtotal = d.Cantidad * d.PrecioCosto
            }).ToListAsync();
        return Json(detalle);
    }

    // POST /Compras/Registrar
    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] CompraDto dto)
    {
        if (dto.Detalles == null || !dto.Detalles.Any())
            return BadRequest(new { mensaje = "Agrega al menos un producto." });

        var compra = new Compra
        {
            Fecha     = DateTime.Now,
            Proveedor = dto.Proveedor,
            Total     = dto.Detalles.Sum(d => d.Cantidad * d.PrecioCosto)
        };
        _db.Compras.Add(compra);
        await _db.SaveChangesAsync();

        foreach (var item in dto.Detalles)
        {
            _db.DetalleCompras.Add(new DetalleCompra
            {
                CompraId    = compra.Id,
                ProductoId  = item.ProductoId,
                Cantidad    = item.Cantidad,
                PrecioCosto = item.PrecioCosto
            });

            var producto = await _db.Productos.FindAsync(item.ProductoId);
            if (producto != null) producto.Stock += item.Cantidad;
        }

        await _db.SaveChangesAsync();
        return Ok(new { mensaje = "Compra registrada", id = compra.Id });
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
