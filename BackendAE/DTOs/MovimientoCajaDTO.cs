using System.Text.Json.Serialization;

namespace BackendAE.DTOs
{
    public class MovimientoCajaDTO
    {
        public int MovimientoCajaId { get; set; }

        public string Tipo { get; set; } = null!;

        public string Concepto { get; set; } = null!;

        public decimal Monto { get; set; }
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime Fecha { get; set; }

        public int CajaSesionId { get; set; }

        public int? UsuarioId { get; set; }
    }
}
