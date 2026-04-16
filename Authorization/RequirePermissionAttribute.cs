// ============================================================
// Authorization/RequirePermissionAttribute.cs
//
// Equivalente al sistema de Gates/Policies de Laravel.
// Uso en Controller:
//   [RequirePermission("ventas.registrar")]
//   public IActionResult Registrar() { ... }
// ============================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventarioApp.Authorization;

/// <summary>
/// Decorador que exige un permiso específico para acceder a una acción.
/// Si el usuario no tiene el permiso, se redirige a "Acceso Denegado".
/// </summary>
public class RequirePermissionAttribute : TypeFilterAttribute
{
    public RequirePermissionAttribute(string permission)
        : base(typeof(PermissionFilter))
    {
        Arguments = new object[] { permission };
    }
}

/// <summary>
/// Filtro interno que verifica el claim "Permission" del usuario.
/// </summary>
public class PermissionFilter : IAuthorizationFilter
{
    private readonly string _permission;

    public PermissionFilter(string permission)
    {
        _permission = permission;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // Si no está autenticado → Login
        if (user.Identity?.IsAuthenticated != true)
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }

        // Si no tiene el permiso → Acceso Denegado (genera ForbidResult que
        // redirige a la ruta configurada en CookieAuthentication.AccessDeniedPath)
        if (!user.HasClaim("Permission", _permission))
        {
            context.Result = new ForbidResult();
        }
    }
}
