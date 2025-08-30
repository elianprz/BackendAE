using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class MovimientoCaja
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovimientoCajaId { get; set; }

        [Required]
        [StringLength(12)]
        public required string Tipo { get; set; }

        [Required]
        [StringLength(120)]
        public required string Concepto { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 2)")]
        public required decimal Monto { get; set; }

        [Required]
        public required DateTime Fecha { get; set; }

        // Clave foránea N:1 con CajaSesion
        [ForeignKey("CajaSesion")]
        public int CajaSesionId { get; set; }
        public CajaSesion? CajaSesion { get; set; }

        // Clave foránea N:1 con Usuario
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}