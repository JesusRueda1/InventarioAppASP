// ============================================================
// Controllers/LoginController.cs
// ============================================================
using InventarioApp.Data;
using InventarioApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventarioApp.Controllers;

public class LoginController : Controller
{
    private readonly ApplicationDbContext _db;
    public LoginController(ApplicationDbContext db) => _db = db;

    // GET /Login
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    // POST /Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        // Buscar usuario por username (insensible a mayúsculas para MySQL)
        var usuario = await _db.Usuarios
            .Include(u => u.Rol)
                .ThenInclude(r => r!.RolPermisos)
                    .ThenInclude(rp => rp.Permiso)
            .FirstOrDefaultAsync(u => u.UserName == model.UserName);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Password, usuario.Password))
        {
            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View(model);
        }

        // Claims base del usuario
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name,           usuario.Nombre),
            new(ClaimTypes.Email,          usuario.Correo),
            new("UserName",                usuario.UserName),
            new(ClaimTypes.Role,           usuario.Rol?.Nombre ?? "Sin Rol")
        };


        // Permisos del rol → cada uno se agrega como claim "Permission"
        // Equivalente a auth()->user()->can() en Laravel
        if (usuario.Rol?.RolPermisos != null)
        {
            foreach (var rp in usuario.Rol.RolPermisos)
            {
                if (rp.Permiso != null)
                    claims.Add(new Claim("Permission", rp.Permiso.Nombre));
            }
        }

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
    }

    // GET /Login/Logout
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index");
    }
}
