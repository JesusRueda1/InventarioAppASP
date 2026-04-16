// ============================================================
// Models/Rol.cs  –  Roles y Permisos del sistema
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApp.Models;

/// <summary>
/// Rol de usuario (Administrador, Vendedor, Bodeguero…).
/// </summary>
[Table("roles")]
public class Rol
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del rol es obligatorio")]
    [StringLength(80)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(255)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    // Navegación
    public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
    public ICollection<Usuario>    Usuarios     { get; set; } = new List<Usuario>();
}

/// <summary>
/// Permiso atómico del sistema (ej. "ventas.registrar").
/// Equivalente a un Gate/Permission de Laravel.
/// </summary>
[Table("permisos")]
public class Permiso
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(255)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    // Navegación
    public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
}

/// <summary>
/// Tabla pivote: asigna permisos a roles.
/// </summary>
[Table("rol_permisos")]
public class RolPermiso
{
    [Column("rol_id")]
    public int RolId { get; set; }

    [ForeignKey("RolId")]
    public Rol? Rol { get; set; }

    [Column("permiso_id")]
    public int PermisoId { get; set; }

    [ForeignKey("PermisoId")]
    public Permiso? Permiso { get; set; }
}
