// ============================================================
// Models/Transaccion.cs  –  Modelo unificado de transacciones
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApp.Models;

public enum TipoTransaccion
{
    Compra,
    Venta
}

public enum EstadoPagoTransaccion
{
    Pendiente,
    Parcial,
    Pagado
}


/// <summary>
/// Cabecera unificada de compras y ventas.
/// El campo Tipo distingue entre Compra y Venta.
/// Los detalles se mantienen en tablas separadas:
///   - detalle_compras (para Tipo = Compra)
///   - detalle_ventas  (para Tipo = Venta)
/// </summary>
[Table("transacciones")]
public class Transaccion
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("tipo")]
    [Display(Name = "Tipo")]
    public TipoTransaccion Tipo { get; set; }

    [Column("fecha")]
    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Column("subtotal", TypeName = "decimal(12,2)")]
    [Display(Name = "Subtotal")]
    public decimal Subtotal { get; set; }

    [Column("total_impuesto", TypeName = "decimal(12,2)")]
    [Display(Name = "Total Impuesto")]
    public decimal TotalImpuesto { get; set; }

    [Column("total", TypeName = "decimal(12,2)")]
    [Display(Name = "Total (Neto)")]
    public decimal Total { get; set; }

    // Estado del Pago en Cartera
    [Column("estado_pago")]
    [Display(Name = "Estado de Pago")]
    public EstadoPagoTransaccion EstadoPago { get; set; } = EstadoPagoTransaccion.Pagado;

    [Column("saldo_pendiente", TypeName = "decimal(12,2)")]
    [Display(Name = "Saldo Pendiente")]
    public decimal SaldoPendiente { get; set; }

    [Column("proveedor")]
    [Display(Name = "Proveedor/Cliente")]
    [StringLength(150)]
    public string? Proveedor { get; set; }

    // FK al usuario que registró la transacción
    [Column("usuario_id")]
    public int? UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public Usuario? Usuario { get; set; }

    // Navegación hacia detalles (separados por tabla, según el Tipo)
    public ICollection<DetalleCompra> DetallesCompra { get; set; } = new List<DetalleCompra>();
    public ICollection<DetalleVenta>  DetallesVenta  { get; set; } = new List<DetalleVenta>();

    // Pagos recibidos asociados a esta factura
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
