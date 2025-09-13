namespace BackendAE.DTOs
{
    public class ProveedorCreacionDTO
    {
        public required string NombreEncargado { get; set; }

        public required string Empresa { get; set; }

        public string? Telefono { get; set; }

        public string? ImagenUrl { get; set; }

        public bool Estado { get; set; }

        public int CatProveedorId { get; set; }
    }
}
