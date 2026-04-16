// ============================================================
// Extensions/ClaimsPrincipalExtensions.cs
//
// Equivalente a auth()->user()->can('permiso') de Laravel.
//
// En Controller:  User.Can("ventas.registrar")      → bool
// En View Razor:  @User.Can("usuarios.ver")         → bool
// En Controller:  int? uid = User.UserId()
// En View Razor:  @User.RoleName()
// ============================================================
using System.Security.Claims;

namespace InventarioApp.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Verifica si el usuario tiene un permiso específico.
    /// Equivalente a auth()->user()->can('permiso') en Laravel.
    /// </summary>
    public static bool Can(this ClaimsPrincipal user, string permission)
        => user.HasClaim("Permission", permission);

    /// <summary>Obtiene el ID del usuario autenticado (null si no autenticado).</summary>
    public static int? UserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(value, out var id) ? id : null;
    }

    /// <summary>Obtiene el nombre del rol del usuario autenticado.</summary>
    public static string RoleName(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Role)?.Value ?? "Sin Rol";
}
