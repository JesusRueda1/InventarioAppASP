// ============================================================
// Models/ViewModels.cs  –  ViewModels de Usuarios y Roles
// ============================================================
using System.ComponentModel.DataAnnotations;

namespace InventarioApp.Models;

// ── ViewModels de Usuarios ───────────────────────────────────

public class UsuarioCreateViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Correo inválido")]
    [StringLength(150)]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona un rol")]
    [Display(Name = "Rol")]
    public int RolId { get; set; }
}

public class UsuarioEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Correo inválido")]
    [StringLength(150)]
    [Display(Name = "Correo electrónico")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona un rol")]
    [Display(Name = "Rol")]
    public int RolId { get; set; }

    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
    [Display(Name = "Nueva contraseña (dejar vacío para no cambiar)")]
    public string? NuevoPassword { get; set; }
}

// ── ViewModels de Roles ──────────────────────────────────────

public class RolEditViewModel
{
    public int RolId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(80)]
    [Display(Name = "Nombre del Rol")]
    public string RolNombre { get; set; } = string.Empty;

    [StringLength(255)]
    [Display(Name = "Descripción")]
    public string? RolDescripcion { get; set; }

    public List<PermisoCheckbox> Permisos { get; set; } = new();
}

public class PermisoCheckbox
{
    public int     PermisoId    { get; set; }
    public string  Nombre       { get; set; } = string.Empty;
    public string? Descripcion  { get; set; }
    public bool    Seleccionado { get; set; }
}
