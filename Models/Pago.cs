// ============================================================
// Models/Pago.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApp.Models;

[Table("pagos")]
public class Pago
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("transaccion_id")]
    public int TransaccionId { get; set; }

    [ForeignKey("TransaccionId")]
    public Transaccion? Transaccion { get; set; }

    [Required]
    [Column("fecha")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required]
    [Column("monto", TypeName = "decimal(12,2)")]
    public decimal Monto { get; set; }

    [Required]
    [StringLength(50)]
    [Column("metodo_pago")]
    [Display(Name = "Método de Pago")]
    public string MetodoPago { get; set; } = "Efectivo";

    [StringLength(100)]
    [Column("referencia")]
    [Display(Name = "Referencia / Notas")]
    public string? Referencia { get; set; }

    // Cajero/Usuario que recibió u originó el pago
    [Column("usuario_id")]
    public int? UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public Usuario? Usuario { get; set; }
}
