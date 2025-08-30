using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class DetalleCompra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetalleCompraId { get; set; }

        [Required]
        public required int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public required decimal PrecioUnitario { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public required decimal Subtotal { get; set; }

        // Clave foránea N:1 con Compra
        public int CompraId { get; set; }
        public Compra? Compra { get; set; }

        // Clave foránea N:1 con Producto
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
    }
}