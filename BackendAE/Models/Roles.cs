using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RolId { get; set; }

        [Required]
        [StringLength(50)]
        public required string RolNombre { get; set; }

        // Relación 1:N con Usuarios. Un Rol puede tener muchos Usuarios.
        public ICollection<Usuario>? Usuarios { get; set; }
    }
}
