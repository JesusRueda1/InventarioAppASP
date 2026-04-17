// ============================================================
// Models/Impuesto.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApp.Models;

[Table("impuestos")]
public class Impuesto
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del impuesto es obligatorio")]
    [StringLength(50)]
    [Column("nombre")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Porcentaje (%)")]
    [Column("porcentaje", TypeName = "decimal(5,2)")]
    public decimal Porcentaje { get; set; }
}
