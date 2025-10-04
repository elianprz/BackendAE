using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class Compra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompraId { get; set; }

        // ---- Información de la compra ----
        [Required]
        public required DateTime FechaCompra { get; set; }

        [StringLength(250)]
        public string? Observacion { get; set; }

        // ---- Campos “copia” del producto (para comparar con Ventas) ----
        [Required]
        [StringLength(120)]
        public required string Nombre { get; set; }

        [StringLength(250)]
        public string? Descripcion { get; set; }

        [Required]
        public required bool Estado { get; set; } = true;   // activo/inactivo

        // Cantidad adquirida (equivalente al “Stock” del producto)
        [Required]
        public required int Stock { get; set; }

        // Precio que pagaste por unidad en esa compra
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public required decimal PrecioAdquisicion { get; set; }

        // Precio de venta que usarás como referencia (puede ser null)
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? PrecioVenta { get; set; }

        // ---- Relación con Proveedor (única relación que mantiene la tabla) ----
        public int ProveedorId { get; set; }

        [ForeignKey(nameof(ProveedorId))]
        public Proveedor? Proveedor { get; set; }

        // ---- Total de la compra (PrecioAdquisicion * Stock) ----
        [Column(TypeName = "decimal(12, 2)")]
        public required decimal Total { get; set; }
    }
}
