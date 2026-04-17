// ============================================================
// Models/MovimientoKardex.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApp.Models;

public enum TipoMovimientoKardex
{
    Ingreso,
    Egreso
}

[Table("movimientos_kardex")]
public class MovimientoKardex
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("producto_id")]
    [Display(Name = "Producto")]
    public int ProductoId { get; set; }

    [ForeignKey("ProductoId")]
    public Producto? Producto { get; set; }

    [Required]
    [Column("fecha")]
    [Display(Name = "Fecha")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required]
    [Column("tipo")]
    [Display(Name = "Tipo")]
    public TipoMovimientoKardex Tipo { get; set; }

    [Required]
    [Column("cantidad")]
    [Display(Name = "Cantidad")]
    public int Cantidad { get; set; }

    [Required]
    [Column("saldo")]
    [Display(Name = "Saldo Resultante")]
    public int Saldo { get; set; }

    // Motivo: "Compra #3", "Venta #12", "Ajuste por merma", etc.
    [Required]
    [StringLength(200)]
    [Column("motivo")]
    [Display(Name = "Motivo")]
    public string Motivo { get; set; } = string.Empty;

    // Relación opcional directa a la tabla unificada de transacciones
    [Column("transaccion_id")]
    public int? TransaccionId { get; set; }

    [ForeignKey("TransaccionId")]
    public Transaccion? Transaccion { get; set; }

    // Quien lo registró
    [Column("usuario_id")]
    public int? UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public Usuario? Usuario { get; set; }
}
