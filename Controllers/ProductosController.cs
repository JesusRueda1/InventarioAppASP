// ============================================================
// Controllers/ProductosController.cs
// ============================================================
using InventarioApp.Data;
using InventarioApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioApp.Controllers;

[Authorize]
public class ProductosController : Controller
{
    private readonly ApplicationDbContext _db;
    public ProductosController(ApplicationDbContext db) => _db = db;

    // GET /Productos
    public async Task<IActionResult> Index()
    {
        // Pasamos las categorías para el <select> del formulario
        ViewBag.Categorias = await _db.Categorias.ToListAsync();
        return View();
    }

    // ── API JSON para Tabulator ──────────────────────────────

    // GET /Productos/ObtenerTodos?busqueda=laptop
    [HttpGet]
    public async Task<IActionResult> ObtenerTodos(string? busqueda)
    {
        // return Json([]);
        var query = _db.Productos.Include(p => p.Categoria).AsQueryable();

        // query
        // Console.WriteLine(query);
        if (!string.IsNullOrWhiteSpace(busqueda))
            query = query.Where(p =>
                p.Nombre.Contains(busqueda) ||
                (p.Descripcion != null && p.Descripcion.Contains(busqueda)));
        var lista = await query.Select(p => new
        {
            p.Id,
            p.Nombre,
            p.Descripcion,
            p.Precio,
            p.Stock,
            p.categoria_id,
            CategoriaNombre = p.Categoria != null ? p.Categoria.Nombre : "—",
            StockBajo = p.Stock < 5
        }).ToListAsync();

        return Json(lista);
    }

    // POST /Productos/Guardar
    [HttpPost]
    public async Task<IActionResult> Guardar([FromBody] Producto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (model.Id == 0)
            _db.Productos.Add(model);
        else
        {
            var existente = await _db.Productos.FindAsync(model.Id);
            if (existente == null) return NotFound();
            existente.Nombre      = model.Nombre;
            existente.Descripcion = model.Descripcion;
            existente.Precio      = model.Precio;
            existente.Stock       = model.Stock;
            existente.categoria_id = model.categoria_id;
        }

        await _db.SaveChangesAsync();
        return Ok(new { mensaje = "Guardado correctamente" });
    }

    // DELETE /Productos/Eliminar/5
    [HttpDelete]
    public async Task<IActionResult> Eliminar(int id)
    {
        var producto = await _db.Productos.FindAsync(id);
        if (producto == null) return NotFound();
        _db.Productos.Remove(producto);
        await _db.SaveChangesAsync();
        return Ok(new { mensaje = "Eliminado correctamente" });
    }

    // GET /Productos/ObtenerPorId/5  (para edición)
    [HttpGet]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var p = await _db.Productos.FindAsync(id);
        if (p == null) return NotFound();
        return Json(p);
    }
}
