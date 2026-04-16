// ============================================================
// Controllers/HomeController.cs  –  Dashboard principal
// ============================================================
using InventarioApp.Data;
using InventarioApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.TotalProductos  = await _db.Productos.CountAsync();
        ViewBag.TotalCategorias = await _db.Categorias.CountAsync();
        ViewBag.StockBajo       = await _db.Productos.CountAsync(p => p.Stock < 5);
        ViewBag.TotalVentas     = await _db.Transacciones
                                           .CountAsync(t => t.Tipo == TipoTransaccion.Venta);
        return View();
    }

    // Página de acceso denegado (redirigido por ForbidResult del PermissionFilter)
    public IActionResult AccesoDenegado()
    {
        return View();
    }
}
