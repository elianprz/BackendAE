using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class Compra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompraId { get; set; }

        [Required]
        public required DateTime FechaCompra { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 2)")]
        public required decimal Total { get; set; }

        [StringLength(250)]
        public string? Observacion { get; set; }

        // Clave foránea N:1 con Proveedor
        public int ProveedorId { get; set; }
        [ForeignKey("ProveedorId")]
        public Proveedor? Proveedor { get; set; }

        // Propiedad de navegación 1:N con DetalleCompra
        public ICollection<DetalleCompra>? DetalleCompras { get; set; }
    }
}