// ============================================================
// Controllers/RolesController.cs
// ============================================================
using InventarioApp.Authorization;
using InventarioApp.Data;
using InventarioApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InventarioApp.Controllers;

[Authorize]
public class RolesController : Controller
{
    private readonly ApplicationDbContext _db;
    public RolesController(ApplicationDbContext db) => _db = db;

    // GET /Roles
    [RequirePermission("roles.ver")]
    public async Task<IActionResult> Index()
    {
        var roles = await _db.Roles
            .Include(r => r.RolPermisos)
                .ThenInclude(rp => rp.Permiso)
            .Include(r => r.Usuarios)
            .OrderBy(r => r.Id)
            .ToListAsync();

        return View(roles);
    }

    // GET /Roles/Edit/1
    [RequirePermission("roles.editar")]
    public async Task<IActionResult> Edit(int id)
    {
        var rol = await _db.Roles
            .Include(r => r.RolPermisos)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rol == null) return NotFound();

        var todosPermisos = await _db.Permisos.OrderBy(p => p.Nombre).ToListAsync();
        var asignados     = rol.RolPermisos.Select(rp => rp.PermisoId).ToHashSet();

        // Agrupar permisos por módulo (ej: "productos", "ventas"…)
        var vm = new RolEditViewModel
        {
            RolId          = rol.Id,
            RolNombre      = rol.Nombre,
            RolDescripcion = rol.Descripcion,
            Permisos       = todosPermisos.Select(p => new PermisoCheckbox
            {
                PermisoId    = p.Id,
                Nombre       = p.Nombre,
                Descripcion  = p.Descripcion,
                Seleccionado = asignados.Contains(p.Id)
            }).ToList()
        };

        return View(vm);
    }

    // POST /Roles/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("roles.editar")]
    public async Task<IActionResult> Edit(int id, RolEditViewModel vm)
    {
        if (id != vm.RolId) return BadRequest();

        var rol = await _db.Roles
            .Include(r => r.RolPermisos)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rol == null) return NotFound();

        // Actualizar nombre y descripción
        rol.Nombre      = vm.RolNombre;
        rol.Descripcion = vm.RolDescripcion;

        // Reconstruir permisos: eliminar existentes y re-insertar los marcados
        _db.RolPermisos.RemoveRange(rol.RolPermisos);
        await _db.SaveChangesAsync();

        var nuevos = (vm.Permisos ?? new())
            .Where(p => p.Seleccionado)
            .Select(p => new RolPermiso { RolId = id, PermisoId = p.PermisoId });

        _db.RolPermisos.AddRange(nuevos);
        await _db.SaveChangesAsync();

        TempData["Exito"] = $"Rol '{rol.Nombre}' actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}


