// ============================================================
// Program.cs  –  Punto de entrada de la aplicación
// ============================================================
using InventarioApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Base de datos ────────────────────────────────────────────
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connStr, ServerVersion.AutoDetect(connStr)));

// ── Autenticación por cookie ─────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath       = "/Login";
        options.LogoutPath      = "/Login/Logout";
        options.AccessDeniedPath = "/Home/AccesoDenegado";
        options.ExpireTimeSpan  = TimeSpan.FromHours(8);
    });

// ── MVC ──────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── Sesión (usada para el carrito del POS) ────────────────────
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

var app = builder.Build();

// ── Pipeline ─────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Ruta por defecto → Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
