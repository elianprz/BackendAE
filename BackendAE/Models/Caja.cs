using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class Caja
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CajaId { get; set; }

        [Required]
        [StringLength(50)]
        public required string Nombre { get; set; }

        [StringLength(150)]
        public string? Descripcion { get; set; }

        [Required]
        public required bool Activa { get; set; }

        // Relación 1:N con CajaSesion
        public ICollection<CajaSesion>? SesionesCaja { get; set; }
    }
}