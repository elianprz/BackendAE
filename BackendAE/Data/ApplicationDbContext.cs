using BackendAE.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para cada una de tus tablas
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CategoriaProducto> CategoriasProductos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<CategoriaProveedor> CategoriasProveedores { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> DetallesCompras { get; set; }
        public DbSet<Caja> Cajas { get; set; }
        public DbSet<CajaSesion> CajaSesiones { get; set; }
        public DbSet<MovimientoCaja> MovimientosCaja { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVentas { get; set; }

        public DbSet<DetalleCompra> DetalleCompras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuraciones especiales de relaciones
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Rol)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RolId);

            modelBuilder.Entity<CajaSesion>()
                .HasOne(cs => cs.UsuarioApertura)
                .WithMany(u => u.SesionesAbiertas)
                .HasForeignKey(cs => cs.UsuarioAperturaId)
                .OnDelete(DeleteBehavior.Restrict); // Evita ciclos de eliminación

            modelBuilder.Entity<CajaSesion>()
                .HasOne(cs => cs.UsuarioCierre)
                .WithMany(u => u.SesionesCerradas)
                .HasForeignKey(cs => cs.UsuarioCierreId)
                .IsRequired(false) // Permite que sea nulo
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.CajaSesion)
                .WithMany(cs => cs.Ventas)
                .HasForeignKey(v => v.CajaSesionId)
                .IsRequired(false) // Permite que sea nulo
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovimientoCaja>()
                .HasOne(mc => mc.CajaSesion)
                .WithMany(cs => cs.MovimientosCaja)
                .HasForeignKey(mc => mc.CajaSesionId);

            // Asegurar unicidad para las propiedades marcadas como 'Unique' en el documento
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Usuario>().HasIndex(u => u.NombreUsuario).IsUnique();
            //modelBuilder.Entity<Usuario>().HasIndex(u => u.NIT).IsUnique();
            //modelBuilder.Entity<Usuario>().HasIndex(u => u.CUI).IsUnique();
            modelBuilder.Entity<Producto>().HasIndex(p => p.SKU).IsUnique();
            modelBuilder.Entity<Venta>().HasIndex(v => v.CodigoVenta).IsUnique();


            // Evita el borrado en cascada para la relación Usuario-CajaSesion
            //modelBuilder.Entity<CajaSesion>()
            //    .HasOne(cs => cs.Usuario)
            //    .WithMany(u => u.CajaSesiones)
            //    .HasForeignKey(cs => cs.UsuarioId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //// Evita el borrado en cascada para la relación Producto-Proveedor
            //modelBuilder.Entity<Producto>()
            //    .HasOne(p => p.Proveedor)
            //    .WithMany(pr => pr.Productos)
            //    .HasForeignKey(p => p.ProveedorId)
            //    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}