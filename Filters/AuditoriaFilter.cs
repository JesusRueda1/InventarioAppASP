// ============================================================
// Filters/AuditoriaFilter.cs  –  Filtro Global de Rastreo ERP
// ============================================================
using InventarioApp.Data;
using InventarioApp.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InventarioApp.Filters;

/// <summary>
/// Intercepta las solicitudes POST/PUT/DELETE y guarda un log automático.
/// Usa su propio DbContext aislado para no interferir con las transacciones del controlador.
/// </summary>
public class AuditoriaFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public AuditoriaFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Ejecutamos la acción original del sistema primero
        var resultContext = await next();

        // Solo nos importan peticiones que modifiquen cosas y que hayan sido exitosas
        var method = context.HttpContext.Request.Method;
        if (method != "POST" && method != "PUT" && method != "DELETE")
            return;

        // Comprobar si hubo una excepción
        if (resultContext.Exception != null)
            return; // No auditar si la acción falló y explotó con una excepción.

        // Evitamos spamear logins
        var controllerName = context.RouteData.Values["controller"]?.ToString() ?? "Desconocido";
        if (controllerName == "Login") return;

        int? usuarioId = null;
        if (context.HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var idClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim != null && int.TryParse(idClaim.Value, out int currentId))
            {
                usuarioId = currentId;
            }
        }

        var actionName = context.RouteData.Values["action"]?.ToString() ?? "Desconocido";
        var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();

        try
        {
            // Usar un DbContext completamente aislado para que no contamine el change tracker del controller
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var log = new AuditoriaLog
            {
                Fecha = DateTime.Now,
                UsuarioId = usuarioId,
                Modulo = controllerName,
                Accion = $"{method} - {actionName}",
                Detalles = $"El usuario interactuó con el módulo {controllerName} ejecutando la acción {actionName}.",
                DireccionIp = ip
            };

            db.AuditoriaLogs.Add(log);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Si la tabla no existe aún o hay otro error de BD, no destruir la operación del usuario
            Console.WriteLine($"[AuditoriaFilter WARN] No se pudo guardar log: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
}
