using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class Venta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VentaId { get; set; }

        [Required]
        public required DateTime FechaVenta { get; set; }

        [Required]
        [StringLength(30)]
        public required string CodigoVenta { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 2)")]
        public required decimal Total { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 2)")]
        public required decimal EfectivoRecibido { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 2)")]
        public required decimal Cambio { get; set; }

        [Required]
        [StringLength(12)]
        public required string EstadoVenta { get; set; }

        // Clave foránea N:1 con Usuario
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        // Clave foránea N:1 con CajaSesion
        [ForeignKey("CajaSesion")]
        public int? CajaSesionId { get; set; }
        public CajaSesion? CajaSesion { get; set; }

        // Propiedad de navegación 1:N con DetalleVenta
        public ICollection<DetalleVenta>? DetalleVentas { get; set; }
    }
}