// ============================================================
// Controllers/ReportesController.cs
// ============================================================
using InventarioApp.Data;
using InventarioApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioApp.Controllers;

[Authorize]
public class ReportesController : Controller
{
    private readonly ApplicationDbContext _db;
    public ReportesController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var model = new ReportesViewModel();

        // 1. Informe de Stock
        var productos = await _db.Productos.ToListAsync();
        model.ValorizacionInventario = productos.Sum(p => p.Stock * p.Precio);
        model.ProductosBajoStock = productos.Count(p => p.Stock < 5);
        model.TotalUnidadesFisicas = productos.Sum(p => p.Stock);

        // 2. Informe Fiscal y Ventas (Últimos 30 días)
        var limitDate = DateTime.Now.AddDays(-30);
        var transaccionesMes = await _db.Transacciones
            .Where(t => t.Fecha >= limitDate)
            .ToListAsync();

        model.VentasTotalesBrutas = transaccionesMes.Where(t => t.Tipo == TipoTransaccion.Venta).Sum(t => t.Total);
        model.IvaPorPagar = transaccionesMes.Where(t => t.Tipo == TipoTransaccion.Venta).Sum(t => t.TotalImpuesto);
        model.ComprasRealizadas = transaccionesMes.Where(t => t.Tipo == TipoTransaccion.Compra).Sum(t => t.Total);
        model.IvaDescontable = transaccionesMes.Where(t => t.Tipo == TipoTransaccion.Compra).Sum(t => t.TotalImpuesto);

        // 3. Estado de la Cartera Global
        model.CuentasPorCobrar = await _db.Transacciones
            .Where(t => t.Tipo == TipoTransaccion.Venta && t.SaldoPendiente > 0)
            .SumAsync(t => t.SaldoPendiente);

        model.CuentasPorPagar = await _db.Transacciones
            .Where(t => t.Tipo == TipoTransaccion.Compra && t.SaldoPendiente > 0)
            .SumAsync(t => t.SaldoPendiente);

        // 4. Informe de Recaudación (Pagos) en los últimos 30 días
        model.RecaudacionEfectivo = await _db.Pagos
            .Where(p => p.Transaccion.Tipo == TipoTransaccion.Venta && p.Fecha >= limitDate)
            .SumAsync(p => p.Monto);

        model.EgresosPagados = await _db.Pagos
            .Where(p => p.Transaccion.Tipo == TipoTransaccion.Compra && p.Fecha >= limitDate)
            .SumAsync(p => p.Monto);

        return View(model);
    }
}

public class ReportesViewModel
{
    // Stock
    public decimal ValorizacionInventario { get; set; }
    public int ProductosBajoStock { get; set; }
    public int TotalUnidadesFisicas { get; set; }

    // Fiscal / Rentabilidad
    public decimal VentasTotalesBrutas { get; set; }
    public decimal IvaPorPagar { get; set; }
    public decimal ComprasRealizadas { get; set; }
    public decimal IvaDescontable { get; set; }

    // Cartera
    public decimal CuentasPorCobrar { get; set; }
    public decimal CuentasPorPagar { get; set; }

    // Liquidez
    public decimal RecaudacionEfectivo { get; set; }
    public decimal EgresosPagados { get; set; }
}
