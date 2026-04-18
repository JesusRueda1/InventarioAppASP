// ============================================================
// Models/LoginViewModel.cs
// ============================================================
using System.ComponentModel.DataAnnotations;

namespace InventarioApp.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    [StringLength(50, MinimumLength = 3)]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
