using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BackendAE.Models
{
    public class CajaSesion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CajaSesionId { get; set; }

        [Required]
        public required DateTime FechaApertura { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 2)")]
        public required decimal MontoApertura { get; set; }
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime? FechaCierre { get; set; }

        [Column(TypeName = "decimal(12, 2)")]
        public decimal MontoCierre { get; set; }

        [Required]
        [StringLength(10)]
        public required string Estado { get; set; }

        [StringLength(250)]
        public string? Observacion { get; set; }

        // Clave foránea N:1 con Caja
        [ForeignKey("Caja")]
        public int CajaId { get; set; }
        public Caja? Caja { get; set; }

        // Claves foráneas N:1 con Usuarios
        [ForeignKey("UsuarioApertura")]
        public int UsuarioAperturaId { get; set; }
        public Usuario? UsuarioApertura { get; set; }

        [ForeignKey("UsuarioCierre")]
        public int? UsuarioCierreId { get; set; }
        public Usuario? UsuarioCierre { get; set; }

        // Propiedad de navegación para la relación 1:N con MovimientoCaja
        public ICollection<MovimientoCaja>? MovimientosCaja { get; set; }

        // Propiedad de navegación para la relación 1:N con Venta
        public ICollection<Venta>? Ventas { get; set; }
    }
}