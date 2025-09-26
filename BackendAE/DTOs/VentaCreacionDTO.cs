// DTOs/VentaCreacionDTO.cs
using System.Collections.Generic;

namespace BackendAE.DTOs
{
    public class VentaCreacionDTO
    {
        public decimal Total { get; set; }
        public decimal EfectivoRecibido { get; set; }
        public decimal Cambio { get; set; }
        public string EstadoVenta { get; set; } = null!;
        public int UsuarioId { get; set; }
        public int? CajaSesionId { get; set; }
        // ✅ Este es el DTO que enviará el Frontend
        public List<VentaDetalleCreacionDTO> DetalleVentas { get; set; } = new();
    }
}