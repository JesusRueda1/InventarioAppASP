// ============================================================
// Models/Producto.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApp.Models;

[Table("productos")]
public class Producto
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(150)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0, 999999.99, ErrorMessage = "Precio inválido")]
    [Display(Name = "Precio")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "El stock es obligatorio")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock no puede ser negativo")]
    [Display(Name = "Stock")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "Selecciona una categoría")]
    [Display(Name = "Categoría")]
    public int categoria_id { get; set; }

    // FK hacia Categoria
    [ForeignKey("categoria_id")]
    public Categoria? Categoria { get; set; }

    // Impuesto que graba este producto
    [Display(Name = "Impuesto")]
    [Column("impuesto_id")]
    public int? ImpuestoId { get; set; }

    [ForeignKey("ImpuestoId")]
    public Impuesto? Impuesto { get; set; }

    // Propiedad calculada: alerta de stock bajo
    [NotMapped]
    public bool StockBajo => Stock < 5;
}
