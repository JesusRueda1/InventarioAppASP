// ============================================================
// Controllers/AuditoriaController.cs  
// ============================================================
using InventarioApp.Authorization;
using InventarioApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioApp.Controllers;

[Authorize]
[RequirePermission("auditoria.ver")]
public class AuditoriaController : Controller
{
    private readonly ApplicationDbContext _db;

    public AuditoriaController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(DateTime? fechaFiltro)
    {
        var query = _db.AuditoriaLogs.Include(a => a.Usuario).AsQueryable();

        if (fechaFiltro.HasValue)
        {
            query = query.Where(a => a.Fecha.Date == fechaFiltro.Value.Date);
        }

        // Top 500 registros para evitar colapsos
        var logs = await query.OrderByDescending(a => a.Fecha).Take(500).ToListAsync();

        ViewBag.FechaFiltro = fechaFiltro?.ToString("yyyy-MM-dd");
        return View(logs);
    }
}
