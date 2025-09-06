// UsuarioCreacionDTO.cs
using System.ComponentModel.DataAnnotations;

namespace BackendAE.DTOs
{
    public class UsuarioCreacionDTO
    {
        [Required]
        public required string PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        [Required]
        public required string PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        // La contraseña se genera en el backend, no se recibe aquí
        // public required string Contrasena { get; set; } 

        public int RolId { get; set; }
        //[Required]
        //public string NombreUsuario { get; set; } = null!;
        public bool Estado { get; set; } = true;
        public string? NIT { get; set; }
        public string? CUI { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }

        
        public DateTime FechaIngreso { get; set; }

        
        public DateTime FechaNacimiento { get; set; }
        public string? Genero { get; set; }
    }
}