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
    [Display(Name = "Tipo")]
    public TipoTransaccion Tipo { get; set; }

    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(12,2)")]
    [Display(Name = "Total")]
    public decimal Total { get; set; }

    [Display(Name = "Proveedor")]
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
}
