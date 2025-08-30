using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class Proveedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProveedorId { get; set; }

        [Required]
        [StringLength(120)]
        public required string NombreEncargado { get; set; }

        [Required]
        [StringLength(120)]
        public required string Empresa { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(250)]
        public string? ImagenUrl { get; set; }

        [Required]
        public required bool Estado { get; set; }

        // Clave foránea N:1 con CategoriaProveedor
        [ForeignKey("CategoriaProveedor")]
        public int CatProveedorId { get; set; }
        public CategoriaProveedor? CategoriaProveedor { get; set; }

        // Relación 1:N
        public ICollection<Compra>? Compras { get; set; }

    }
}