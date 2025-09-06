namespace BackendAE.DTOs
{
    public class UsuarioActualizacionDTO
    {
        public string? PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string? PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        public string? Email { get; set; }
        public int? RolId { get; set; }
        public string? NombreUsuario { get; set; }
        public bool? Estado { get; set; }
        public string? NIT { get; set; }
        public string? CUI { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        
        public DateTime? FechaNacimiento { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public string? Genero { get; set; }
        
        //public DateTime? FechaIngreso { get; set; } = DateTime.Now; // Por defecto, la fecha de ingreso es la fecha actual
    }
}
