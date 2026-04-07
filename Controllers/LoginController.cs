// ============================================================
// Controllers/LoginController.cs
// ============================================================
using InventarioApp.Data;
using InventarioApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventarioApp.Controllers;

public class LoginController : Controller
{
    private readonly ApplicationDbContext _db;

    public LoginController(ApplicationDbContext db) => _db = db;

    // GET /Login
    public IActionResult Index()
    {
        // Si ya está autenticado, ir al Dashboard
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

        var usuario = _db.Usuarios.FirstOrDefault(u => u.Correo == model.Correo);

        // Validar usuario y contraseña
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Password, usuario.Password))
        {
            ModelState.AddModelError("", "Correo o contraseña incorrectos.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name,           usuario.Nombre),
            new(ClaimTypes.Email,          usuario.Correo)
        };

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

    // // GET /Login/Seed  ← BORRAR después de usarlo
    // public IActionResult Seed()
    // {
    //     var hash = BCrypt.Net.BCrypt.HashPassword("Admin123");
    //     var usuario = _db.Usuarios.FirstOrDefault(u => u.Correo == "admin@demo.com");
    //     if (usuario != null)
    //     {
    //         usuario.Password = hash;
    //         _db.SaveChanges();
    //         return Content($"Hash actualizado: {hash}");
    //     }
    //     return Content("Usuario no encontrado");
    // }
}
