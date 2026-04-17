// ============================================================
// Data/ApplicationDbContext.cs
// ============================================================
using InventarioApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioApp.Data;

/// <summary>
/// Contexto principal de Entity Framework Core.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ── Catálogos ─────────────────────────────────────────────
    public DbSet<Usuario>       Usuarios       { get; set; }
    public DbSet<Categoria>     Categorias     { get; set; }
    public DbSet<Producto>      Productos      { get; set; }
    public DbSet<Impuesto>      Impuestos      { get; set; }

    // ── Roles y Permisos ──────────────────────────────────────
    public DbSet<Rol>           Roles          { get; set; }
    public DbSet<Permiso>       Permisos       { get; set; }
    public DbSet<RolPermiso>    RolPermisos    { get; set; }

    // ── Transacciones (unifica Compras + Ventas) ──────────────
    public DbSet<Transaccion>   Transacciones  { get; set; }
    public DbSet<DetalleCompra> DetalleCompras { get; set; }
    public DbSet<DetalleVenta>  DetalleVentas  { get; set; }

    // ── ERP (Kardex, Cuentas, Actividad) ──────────────────────
    public DbSet<MovimientoKardex> MovimientosKardex { get; set; }
    public DbSet<Pago>             Pagos             { get; set; }
    public DbSet<AuditoriaLog>     AuditoriaLogs     { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Índice único en correo de usuario ─────────────────
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Correo)
            .IsUnique();

        // ── Un producto → una categoría (restricción de borrado) ──
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.categoria_id)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Producto → Impuesto ────────────────────────────────
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Impuesto)
            .WithMany()
            .HasForeignKey(p => p.ImpuestoId)
            .OnDelete(DeleteBehavior.SetNull);

        // ── Transaccion: guardar enum como string ─────────────
        modelBuilder.Entity<Transaccion>()
            .Property(t => t.Tipo)
            .HasConversion<string>()
            .HasMaxLength(15);

        modelBuilder.Entity<Transaccion>()
            .Property(t => t.EstadoPago)
            .HasConversion<string>()
            .HasMaxLength(15);

        // ── Kardex: guardar enum como string ─────────────
        modelBuilder.Entity<MovimientoKardex>()
            .Property(m => m.Tipo)
            .HasConversion<string>()
            .HasMaxLength(15);

        // ── Usuario → Rol ──────────────────────────────────────
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RolId)
            .OnDelete(DeleteBehavior.SetNull);

        // ── RolPermiso: llave primaria compuesta ───────────────
        modelBuilder.Entity<RolPermiso>()
            .HasKey(rp => new { rp.RolId, rp.PermisoId });

        // ── Transaccion → DetalleCompra ────────────────────────
        modelBuilder.Entity<DetalleCompra>()
            .HasOne(d => d.Transaccion)
            .WithMany(t => t.DetallesCompra)
            .HasForeignKey(d => d.TransaccionId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Transaccion → DetalleVenta ─────────────────────────
        modelBuilder.Entity<DetalleVenta>()
            .HasOne(d => d.Transaccion)
            .WithMany(t => t.DetallesVenta)
            .HasForeignKey(d => d.TransaccionId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Transaccion → Pagos ────────────────────────────────
        modelBuilder.Entity<Pago>()
            .HasOne(p => p.Transaccion)
            .WithMany(t => t.Pagos)
            .HasForeignKey(p => p.TransaccionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
