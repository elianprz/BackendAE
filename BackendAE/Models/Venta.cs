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

        // ------------------- NUEVO -------------------
        // FK opcional al producto vendido (ahora **nullable**)
        public int? ProductoId { get; set; }

        [ForeignKey(nameof(ProductoId))]
        public Producto? Producto { get; set; }

        public int CantidadVendida { get; set; }          // cantidad vendida
        public decimal PrecioUnitario { get; set; }       // precio por unidad
        public decimal SubTotal { get; set; }             // CantidadVendida * PrecioUnitario

        // -----------------------------------------------------------------
        //  Relaciones con Usuario y CajaSesion (siguen igual)
        // -----------------------------------------------------------------
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [ForeignKey("CajaSesion")]
        public int? CajaSesionId { get; set; }
        public CajaSesion? CajaSesion { get; set; }
    }
}
