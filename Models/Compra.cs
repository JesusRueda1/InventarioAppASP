// ============================================================
// Models/Compra.cs  –  Todos los modelos de transacciones
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApp.Models
{
    // ── Cabecera de una compra (entrada de stock) ────────────
    [Table("compras")]
    public class Compra
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Display(Name = "Proveedor")]
        [StringLength(150)]
        public string? Proveedor { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        public ICollection<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>();
    }

    // ── Línea de detalle de compra ───────────────────────────
    [Table("detalle_compras")]
    public class DetalleCompra
    {
        [Key]
        public int Id { get; set; }

        [Column("compra_id")]
        public int CompraId { get; set; }
        [ForeignKey("CompraId")]
        public Compra? Compra { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Column("precio_costo", TypeName = "decimal(10,2)")]
        public decimal PrecioCosto { get; set; }
    }

    // ── Cabecera de una venta (POS) ──────────────────────────
    [Table("ventas")]
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(12,2)")]
        [Display(Name = "Total")]
        public decimal Total { get; set; }

        public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }

    // ── Línea de detalle de venta ────────────────────────────
    [Table("detalle_ventas")]
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }

        [Column("venta_id")]
        public int VentaId { get; set; }
        [ForeignKey("VentaId")]
        public Venta? Venta { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Cantidad { get; set; }

        [Column("precio_venta", TypeName = "decimal(10,2)")]
        public decimal PrecioVenta { get; set; }
    }

    // ── ViewModel para el formulario de Login ────────────────
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
