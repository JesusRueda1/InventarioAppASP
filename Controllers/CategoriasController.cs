// ============================================================
// Controllers/CategoriasController.cs
// ============================================================
using InventarioApp.Data;
using InventarioApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioApp.Controllers;

[Authorize]
public class CategoriasController : Controller
{
    private readonly ApplicationDbContext _db;
    public CategoriasController(ApplicationDbContext db) => _db = db;

    // GET /Categorias  – vista principal con Tabulator
    public IActionResult Index() => View();

    // ── API JSON para Tabulator ──────────────────────────────

    // GET /Categorias/ObtenerTodas
    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var lista = await _db.Categorias
            .Select(c => new { c.Id, c.Nombre })
            .ToListAsync();
        return Json(lista);
    }

    // POST /Categorias/Guardar  (crear o editar)
    [HttpPost]
    public async Task<IActionResult> Guardar([FromBody] Categoria model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (model.Id == 0)
            _db.Categorias.Add(model);
        else
        {
            var existente = await _db.Categorias.FindAsync(model.Id);
            if (existente == null) return NotFound();
            existente.Nombre = model.Nombre;
        }

        await _db.SaveChangesAsync();
        return Ok(new { mensaje = "Guardado correctamente" });
    }

    // DELETE /Categorias/Eliminar/5
    [HttpDelete]
    public async Task<IActionResult> Eliminar(int id)
    {
        var categoria = await _db.Categorias.FindAsync(id);
        if (categoria == null) return NotFound();

        // Verificar que no tenga productos asociados
        bool tieneProductos = await _db.Productos.AnyAsync(p => p.categoria_id == id);
        if (tieneProductos)
            return BadRequest(new { mensaje = "No se puede eliminar: tiene productos asociados." });

        _db.Categorias.Remove(categoria);
        await _db.SaveChangesAsync();
        return Ok(new { mensaje = "Eliminado correctamente" });
    }
}
