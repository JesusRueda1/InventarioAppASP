// ============================================================
// Models/Compra.cs  –  Detalles de compras y ventas
// Los encabezados (antes Compra/Venta) ahora están en Transaccion.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApp.Models
{
    // ── Línea de detalle de compra ───────────────────────────
    // La FK apunta a transacciones.id (antes era compras.id)
    [Table("detalle_compras")]
    public class DetalleCompra
    {
        [Key]
        public int Id { get; set; }

        [Column("transaccion_id")]
        public int TransaccionId { get; set; }

        [ForeignKey("TransaccionId")]
        public Transaccion? Transaccion { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio_costo", TypeName = "decimal(10,2)")]
        public decimal PrecioCosto { get; set; }

        // Impuestos históricos al momento de la compra
        [Column("porcentaje_impuesto", TypeName = "decimal(5,2)")]
        public decimal PorcentajeImpuesto { get; set; }

        [Column("monto_impuesto", TypeName = "decimal(10,2)")]
        public decimal MontoImpuesto { get; set; }
    }

    // ── Línea de detalle de venta ────────────────────────────
    // La FK apunta a transacciones.id (antes era ventas.id)
    [Table("detalle_ventas")]
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }

        [Column("transaccion_id")]
        public int TransaccionId { get; set; }

        [ForeignKey("TransaccionId")]
        public Transaccion? Transaccion { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio_venta", TypeName = "decimal(10,2)")]
        public decimal PrecioVenta { get; set; }

        // Impuestos históricos al momento de la venta
        [Column("porcentaje_impuesto", TypeName = "decimal(5,2)")]
        public decimal PorcentajeImpuesto { get; set; }

        [Column("monto_impuesto", TypeName = "decimal(10,2)")]
        public decimal MontoImpuesto { get; set; }
    }
}
