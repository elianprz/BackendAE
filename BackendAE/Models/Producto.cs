using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class Producto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductoId { get; set; }

        [Required]
        [StringLength(120)]
        public required string Nombre { get; set; }

        [StringLength(250)]
        public string? Descripcion { get; set; }

        [Required]
        public required bool Estado { get; set; }

        [Required]
        public required int Stock { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public required decimal PrecioAdquisicion { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public required decimal PrecioVenta { get; set; }

        [StringLength(250)]
        public string? ImagenUrl { get; set; }

        [StringLength(40)]
        public string? SKU { get; set; }

        // Clave foránea N:1 con CategoriaProducto
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public CategoriaProducto? CategoriaProducto { get; set; }
        // Relaciones 1:N con DetalleCompra y DetalleVenta
        public ICollection<DetalleCompra>? DetallesCompras { get; set; }
        public ICollection<DetalleVenta>? DetallesVentas { get; set; }
    }
}