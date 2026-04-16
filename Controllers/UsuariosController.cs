// ============================================================
// Controllers/UsuariosController.cs
// ============================================================
using InventarioApp.Authorization;
using InventarioApp.Data;
using InventarioApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InventarioApp.Controllers;

[Authorize]
public class UsuariosController : Controller
{
    private readonly ApplicationDbContext _db;
    public UsuariosController(ApplicationDbContext db) => _db = db;

    // GET /Usuarios
    [RequirePermission("usuarios.ver")]
    public IActionResult Index() => View();

    // GET /Usuarios/ObtenerTodos  (AJAX para Tabulator)
    [HttpGet]
    [RequirePermission("usuarios.ver")]
    public async Task<IActionResult> ObtenerTodos()
    {
        var lista = await _db.Usuarios
            .Include(u => u.Rol)
            .OrderBy(u => u.Nombre)
            .Select(u => new
            {
                u.Id,
                u.Nombre,
                u.Correo,
                Rol = u.Rol != null ? u.Rol.Nombre : "Sin Rol"
            })
            .ToListAsync();

        return Json(lista);
    }

    // GET /Usuarios/Create
    [RequirePermission("usuarios.crear")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = new SelectList(await _db.Roles.OrderBy(r => r.Nombre).ToListAsync(), "Id", "Nombre");
        return View();
    }

    // POST /Usuarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("usuarios.crear")]
    public async Task<IActionResult> Create(UsuarioCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(await _db.Roles.OrderBy(r => r.Nombre).ToListAsync(), "Id", "Nombre");
            return View(vm);
        }

        if (await _db.Usuarios.AnyAsync(u => u.Correo == vm.Correo))
        {
            ModelState.AddModelError("Correo", "Este correo ya está registrado.");
            ViewBag.Roles = new SelectList(await _db.Roles.OrderBy(r => r.Nombre).ToListAsync(), "Id", "Nombre");
            return View(vm);
        }

        _db.Usuarios.Add(new Usuario
        {
            Nombre   = vm.Nombre,
            Correo   = vm.Correo,
            Password = BCrypt.Net.BCrypt.HashPassword(vm.Password),
            RolId    = vm.RolId
        });

        await _db.SaveChangesAsync();
        TempData["Exito"] = $"Usuario '{vm.Nombre}' creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET /Usuarios/Edit/5
    [RequirePermission("usuarios.editar")]
    public async Task<IActionResult> Edit(int id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        ViewBag.Roles = new SelectList(await _db.Roles.OrderBy(r => r.Nombre).ToListAsync(), "Id", "Nombre", usuario.RolId);

        return View(new UsuarioEditViewModel
        {
            Id     = usuario.Id,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            RolId  = usuario.RolId ?? 0
        });
    }

    // POST /Usuarios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("usuarios.editar")]
    public async Task<IActionResult> Edit(int id, UsuarioEditViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(await _db.Roles.OrderBy(r => r.Nombre).ToListAsync(), "Id", "Nombre", vm.RolId);
            return View(vm);
        }

        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        if (await _db.Usuarios.AnyAsync(u => u.Correo == vm.Correo && u.Id != id))
        {
            ModelState.AddModelError("Correo", "Este correo ya está en uso.");
            ViewBag.Roles = new SelectList(await _db.Roles.OrderBy(r => r.Nombre).ToListAsync(), "Id", "Nombre", vm.RolId);
            return View(vm);
        }

        usuario.Nombre = vm.Nombre;
        usuario.Correo = vm.Correo;
        usuario.RolId  = vm.RolId;

        if (!string.IsNullOrWhiteSpace(vm.NuevoPassword))
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(vm.NuevoPassword);

        await _db.SaveChangesAsync();
        TempData["Exito"] = $"Usuario '{vm.Nombre}' actualizado.";
        return RedirectToAction(nameof(Index));
    }

    // POST /Usuarios/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("usuarios.eliminar")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await _db.Usuarios.FindAsync(id);
        if (usuario == null)
            return Json(new { ok = false, mensaje = "Usuario no encontrado." });

        _db.Usuarios.Remove(usuario);
        await _db.SaveChangesAsync();
        return Json(new { ok = true, mensaje = $"Usuario '{usuario.Nombre}' eliminado." });
    }
}


