using System.ComponentModel.DataAnnotations;

namespace BackendAE.DTOs
{
    public class CambioContrasenaDTO
    {
        [Required]
        public required string ContrasenaActual { get; set; }
        [Required]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public required string NuevaContrasena { get; set; }
    }
}
