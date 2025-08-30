using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class CategoriaProveedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CatProveedorId { get; set; }

        [Required]
        [StringLength(80)]
        public required string Nombre { get; set; }

        [StringLength(200)]
        public string? Descripcion { get; set; }

        [Required]
        public required bool Estado { get; set; }

        // Relación 1:N con Proveedores
        public ICollection<Proveedor>? Proveedores { get; set; }
    }
}
