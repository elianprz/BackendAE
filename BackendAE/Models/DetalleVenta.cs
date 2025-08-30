using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class DetalleVenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DetalleVentaId { get; set; }

        [Required]
        public required int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public required decimal PrecioUnitario { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public required decimal Subtotal { get; set; }

        // Clave foránea N:1 con Venta
        [ForeignKey("Venta")]
        public int VentaId { get; set; }
        public Venta? Venta { get; set; }

        // Clave foránea N:1 con Producto
        [ForeignKey("Producto")]
        public int ProductoId { get; set; }
        public Producto? Producto { get; set; }
    }
}