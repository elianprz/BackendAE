namespace BackendAE.DTOs
{
    public class UsuarioDTO
    {
        public int UsuarioId { get; set; }

        public string PrimerNombre { get; set; } = null!;

        public string? SegundoNombre { get; set; }

        public string PrimerApellido { get; set; } = null!;

        public string? SegundoApellido { get; set; }

        public string? NIT { get; set; }

        public string? CUI { get; set; }

        public DateTime? FechaIngreso { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        public string? Telefono { get; set; }

        public string? Direccion { get; set; }

        public string? Genero { get; set; }

        public bool Estado { get; set; }

        public string Email { get; set; } = null!;

        public string NombreUsuario { get; set; } = null!;

        public int RolId { get; set; }

        public string? RolNombre { get; set; } // Opcional, si deseas mostrar el nombre del rol
    }
}
