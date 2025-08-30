using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BackendAE.DTOs
{
    public class CajaSesionCreacionDTO
    {
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public required DateTime FechaApertura { get; set; } = DateTime.Now;
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El monto de apertura no puede ser un valor negativo.")]
        public required decimal MontoApertura { get; set; }
        [JsonConverter(typeof(DateTimeJsonConverter))]
        public DateTime? FechaCierre { get; set; }

        public required decimal MontoCierre { get; set; }

        public bool Estado { get; set; } = true;
        public string? Observacion { get; set; }

        [Required]
        public required int CajaId { get; set; }

        [Required]
        public required int UsuarioAperturaId { get; set; }

        public required int UsuarioCierreId { get; set; }

        //public int? UsuarioId { get; set; }
    }
}