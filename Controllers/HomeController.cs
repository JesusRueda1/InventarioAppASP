// ============================================================
// Controllers/HomeController.cs  –  Dashboard principal
// ============================================================
using InventarioApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioApp.Controllers;

[Authorize]   // Sólo usuarios autenticados
public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        // Datos para las tarjetas del dashboard
        ViewBag.TotalProductos  = await _db.Productos.CountAsync();
        ViewBag.TotalCategorias = await _db.Categorias.CountAsync();
        ViewBag.StockBajo       = await _db.Productos.CountAsync(p => p.Stock < 5);
        ViewBag.TotalVentas     = await _db.Ventas.CountAsync();

        return View();
    }
}
