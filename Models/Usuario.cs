// ============================================================
// Models/Usuario.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApp.Models;

[Table("usuarios")]
public class Usuario
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El username debe tener entre 3 y 50 caracteres")]
    [RegularExpression(@"^[a-zA-Z0-9_\.]+$", ErrorMessage = "Solo letras, números, puntos y guiones bajos")]
    [Column("username")]
    [Display(Name = "Nombre de usuario")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress]
    [StringLength(150)]
    [Display(Name = "Correo")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(255)]
    public string Password { get; set; } = string.Empty;

    // FK al rol asignado
    [Column("rol_id")]
    [Display(Name = "Rol")]
    public int? RolId { get; set; }

    [ForeignKey("RolId")]
    public Rol? Rol { get; set; }
}
