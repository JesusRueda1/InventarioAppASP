// ============================================================
// Models/AuditoriaLog.cs
// ============================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioApp.Models;

[Table("auditoria_logs")]
public class AuditoriaLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("fecha")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Column("usuario_id")]
    public int? UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public Usuario? Usuario { get; set; }

    [Required]
    [StringLength(100)]
    [Column("modulo")]
    public string Modulo { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Column("accion")]
    public string Accion { get; set; } = string.Empty;

    [Required]
    [Column("detalles")]
    public string Detalles { get; set; } = string.Empty;

    [StringLength(50)]
    [Column("direccion_ip")]
    public string? DireccionIp { get; set; }
}
