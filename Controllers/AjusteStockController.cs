// ============================================================
// Controllers/AjusteStockController.cs
// ============================================================
using InventarioApp.Authorization;
using InventarioApp.Data;
using InventarioApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventarioApp.Controllers;

[Authorize]
[RequirePermission("kardex.ajustar")]
public class AjusteStockController : Controller
{
    private readonly ApplicationDbContext _db;
    public AjusteStockController(ApplicationDbContext db) => _db = db;

    // GET /AjusteStock
    public async Task<IActionResult> Index()
    {
        ViewBag.Productos = await _db.Productos.OrderBy(p => p.Nombre).ToListAsync();
        
        // Cargar últimos 50 movimientos kardex de ajustes manuales o en general
        var kardex = await _db.MovimientosKardex
            .Include(m => m.Producto)
            .Include(m => m.Usuario)
            .OrderByDescending(m => m.Fecha)
            .Take(50)
            .ToListAsync();
            
        return View(kardex);
    }

    // POST /AjusteStock/Guardar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(int productoId, int cantidadAjuste, TipoMovimientoKardex tipo, string motivo)
    {
        if (cantidadAjuste <= 0 || string.IsNullOrWhiteSpace(motivo))
        {
            TempData["Error"] = "La cantidad debe ser mayor a cero y debes proveer un motivo justificado.";
            return RedirectToAction(nameof(Index));
        }

        var producto = await _db.Productos.FindAsync(productoId);
        if (producto == null)
        {
            TempData["Error"] = "Producto no encontrado.";
            return RedirectToAction(nameof(Index));
        }

        // Si es un egreso, validar que haya stock suficiente (opcional, pero recomendado)
        if (tipo == TipoMovimientoKardex.Egreso && producto.Stock < cantidadAjuste)
        {
            TempData["Error"] = $"No hay suficiente stock para descontar. Saldo actual: {producto.Stock}.";
            return RedirectToAction(nameof(Index));
        }

        // Procedemos a actualizar el stock maestro
        if (tipo == TipoMovimientoKardex.Ingreso)
            producto.Stock += cantidadAjuste;
        else
            producto.Stock -= cantidadAjuste;

        int? userId = null;
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (idClaim != null && int.TryParse(idClaim.Value, out int currentId))
            userId = currentId;

        // Inyectamos el acta en el Kardex
        var kardex = new MovimientoKardex
        {
            ProductoId = producto.Id,
            Fecha = DateTime.Now,
            Tipo = tipo,
            Cantidad = cantidadAjuste,
            Saldo = producto.Stock,
            Motivo = $"Ajuste Manuel: {motivo}",
            UsuarioId = userId
        };

        _db.MovimientosKardex.Add(kardex);
        await _db.SaveChangesAsync();

        TempData["Exito"] = $"Kardex actualizado correctamente. El nuevo stock de {producto.Nombre} es {producto.Stock}.";
        return RedirectToAction(nameof(Index));
    }
}
