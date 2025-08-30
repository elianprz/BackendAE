using System.ComponentModel.DataAnnotations;

namespace BackendAE.DTOs
{
    public class DetalleVentaCreacionDTO
    {
        [Required]
        public required int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public required int Cantidad { get; set; }

        // Puedes permitir PrecioUnitario y Subtotal si lo calculas tú desde frontend
        public decimal? PrecioUnitario { get; set; }

        public decimal? Subtotal { get; set; }
    }
}
