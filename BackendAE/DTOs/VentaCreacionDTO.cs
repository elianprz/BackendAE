using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackendAE.DTOs
{
    public class VentaCreacionDTO
    {
        
        public DateTime FechaVenta { get; set; }

        //public string CodigoVenta { get; set; } = null!;

        public decimal Total { get; set; }

        public decimal EfectivoRecibido { get; set; }

        public decimal Cambio { get; set; }

        public string EstadoVenta { get; set; } = null!;

        public int UsuarioId { get; set; }

        public int? CajaSesionId { get; set; }

        public List<DetalleVentaDTO> DetalleVentas { get; set; } = new();
    }
}
