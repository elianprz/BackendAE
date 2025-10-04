using BackendAE.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // ------------------- Tablas -------------------
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CategoriaProducto> CategoriasProductos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<CategoriaProveedor> CategoriasProveedores { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }

        public DbSet<Compra> Compras { get; set; }          // ← sin detalle
        public DbSet<Venta> Ventas { get; set; }            // ← sin detalle

        public DbSet<Caja> Cajas { get; set; }
        public DbSet<CajaSesion> CajaSesiones { get; set; }
        public DbSet<MovimientoCaja> MovimientosCaja { get; set; }

        // -------------------------------------------------
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ------- Relaciones ya existentes -------------
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId);

            modelBuilder.Entity<CajaSesion>()
                .HasOne(cs => cs.UsuarioApertura)
                .WithMany(u => u.SesionesAbiertas)
                .HasForeignKey(cs => cs.UsuarioAperturaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CajaSesion>()
                .HasOne(cs => cs.UsuarioCierre)
                .WithMany(u => u.SesionesCerradas)
                .HasForeignKey(cs => cs.UsuarioCierreId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.CajaSesion)
                .WithMany(cs => cs.Ventas)
                .HasForeignKey(v => v.CajaSesionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venta>()
                 .HasOne(v => v.Producto)
                 .WithMany()
                 .HasForeignKey(v => v.ProductoId)
                 .OnDelete(DeleteBehavior.Restrict);   // NO cascade

            modelBuilder.Entity<MovimientoCaja>()
                .HasOne(mc => mc.CajaSesion)
                .WithMany(cs => cs.MovimientosCaja)
                .HasForeignKey(mc => mc.CajaSesionId);

            // ------- Índices de unicidad -------
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NombreUsuario).IsUnique();
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.SKU).IsUnique();
            //modelBuilder.Entity<Venta>()
            //    .HasIndex(v => v.CodigoVenta).IsUnique();


            // Configuración explícita para propiedades 'decimal' de 'Venta'
            modelBuilder.Entity<Venta>()
                .Property(v => v.PrecioUnitario)
                .HasPrecision(12, 2); // Ajusta la precisión y escala (p, s) según tus necesidades

            modelBuilder.Entity<Venta>()
                .Property(v => v.SubTotal)
                .HasPrecision(12, 2); // Ajusta la precisión y escala (p, s) según tus necesidades


            // -------------------------------------------------
            //  No hay FK a Producto para Compra (campo copiado)
            // -------------------------------------------------
        }
    }
}
