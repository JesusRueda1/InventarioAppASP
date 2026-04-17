// ============================================================
// Controllers/PagosController.cs
// ============================================================
using InventarioApp.Data;
using InventarioApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioApp.Controllers;

[Authorize]
public class PagosController : Controller
{
    private readonly ApplicationDbContext _db;
    public PagosController(ApplicationDbContext db) => _db = db;

    // GET /Pagos
    // Muestra el resumen de deudas pendientes y transacciones globales
    public async Task<IActionResult> Index()
    {
        // Traer todas las transacciones pendientes (Cuentas por Cobrar y por Pagar)
        var cartera = await _db.Transacciones
            .Where(t => t.EstadoPago == EstadoPagoTransaccion.Pendiente || t.EstadoPago == EstadoPagoTransaccion.Parcial)
            .OrderBy(t => t.Fecha)
            .ToListAsync();

        return View(cartera);
    }

    // GET /Pagos/DetalleTransaccion/5
    [HttpGet]
    public async Task<IActionResult> DetalleTransaccion(int id)
    {
        var transaccion = await _db.Transacciones
            .Include(t => t.Pagos)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaccion == null) return NotFound();

        return Json(new {
            transaccion.Id,
            Tipo = transaccion.Tipo.ToString(),
            transaccion.Total,
            transaccion.SaldoPendiente,
            EstadoPago = transaccion.EstadoPago.ToString(),
            Proveedor = transaccion.Proveedor ?? "Cliente Final (POS)",
            Pagos = transaccion.Pagos.Select(p => new {
                p.Id,
                Fecha = p.Fecha.ToString("dd/MM/yyyy HH:mm"),
                p.Monto,
                p.MetodoPago
            })
        });
    }

    // POST /Pagos/Abonar
    [HttpPost]
    public async Task<IActionResult> Abonar([FromBody] AbonoDto dto)
    {
        if (dto.Monto <= 0)
            return BadRequest(new { mensaje = "El monto a abonar debe ser mayor a cero." });

        var transaccion = await _db.Transacciones.FindAsync(dto.TransaccionId);
        if (transaccion == null) 
            return NotFound(new { mensaje = "Transacción no válida." });

        if (transaccion.EstadoPago == EstadoPagoTransaccion.Pagado || transaccion.SaldoPendiente <= 0)
            return BadRequest(new { mensaje = "Esta transacción ya se encuentra totalmente saldada." });

        if (dto.Monto > transaccion.SaldoPendiente)
            return BadRequest(new { mensaje = $"El abono supera la deuda restante. Máximo permitido: ${transaccion.SaldoPendiente}" });

        // Crear el pago
        var pago = new Pago
        {
            TransaccionId = transaccion.Id,
            Monto = dto.Monto,
            Fecha = DateTime.Now,
            MetodoPago = dto.MetodoPago ?? "Efectivo"
        };
        _db.Pagos.Add(pago);

        // Descontar saldo y actualizar estado automáticamente
        transaccion.SaldoPendiente -= dto.Monto;

        if (transaccion.SaldoPendiente == 0)
            transaccion.EstadoPago = EstadoPagoTransaccion.Pagado;
        else
            transaccion.EstadoPago = EstadoPagoTransaccion.Parcial;

        await _db.SaveChangesAsync();

        return Ok(new { 
            mensaje = "Abono acreditado exitosamente.", 
            saldoRestante = transaccion.SaldoPendiente,
            nuevoEstado = transaccion.EstadoPago.ToString()
        });
    }
}

public class AbonoDto
{
    public int TransaccionId { get; set; }
    public decimal Monto { get; set; }
    public string? MetodoPago { get; set; }
}
