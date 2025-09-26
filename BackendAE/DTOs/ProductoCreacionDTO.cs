using System.ComponentModel.DataAnnotations;

namespace BackendAE.DTOs
{
    public class ProductoCreacionDTO
    {
        public int ProductoId { get; set; }
        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(120, ErrorMessage = "El nombre no puede exceder los 120 caracteres.")]
        public required string Nombre { get; set; }

        [StringLength(250, ErrorMessage = "La descripción no puede exceder los 250 caracteres.")]
        public string? Descripcion { get; set; }

        public bool Estado { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser un valor negativo.")]
        public required int Stock { get; set; }

        [Required(ErrorMessage = "El precio de adquisición es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de adquisición debe ser mayor a cero.")]
        public required decimal PrecioAdquisicion { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio de venta debe ser mayor a cero.")]
        public required decimal PrecioVenta { get; set; }

        [StringLength(250, ErrorMessage = "La URL de la imagen no puede exceder los 250 caracteres.")]
        public string? ImagenUrl { get; set; }

        //[StringLength(40, ErrorMessage = "El SKU no puede exceder los 40 caracteres.")]
        //public string? SKU { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        public required int CategoriaId { get; set; }
    }
}