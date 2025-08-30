using System.Text.Json.Serialization;

namespace BackendAE.DTOs
{
    public class CompraCreacionDTO
    {
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public required DateTime FechaCompra { get; set; }

        public string? Observacion { get; set; }

        public decimal Total { get; set; }

        public int ProveedorId { get; set; }

        public required List<DetalleCompraCreacionDTO> DetalleCompras { get; set; }
    }
}
