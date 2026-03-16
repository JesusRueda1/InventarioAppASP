// ============================================================
// Data/ApplicationDbContext.cs
// ============================================================
using InventarioApp.Models;
using Microsoft.EntityFrameworkCore;

namespace InventarioApp.Data;

/// <summary>
/// Contexto principal de Entity Framework Core.
/// Cada DbSet representa una tabla en la base de datos.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // ── Tablas ───────────────────────────────────────────────
    public DbSet<Usuario>       Usuarios       { get; set; }
    public DbSet<Categoria>     Categorias     { get; set; }
    public DbSet<Producto>      Productos      { get; set; }
    public DbSet<Compra>        Compras        { get; set; }
    public DbSet<DetalleCompra> DetalleCompras { get; set; }
    public DbSet<Venta>         Ventas         { get; set; }
    public DbSet<DetalleVenta>  DetalleVentas  { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Índice único en correo de usuario
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Correo)
            .IsUnique();

        // Un producto → una categoría (no borrar categoría con productos)
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.categoria_id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
