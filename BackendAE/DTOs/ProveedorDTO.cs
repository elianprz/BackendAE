namespace BackendAE.DTOs
{
    public class ProveedorDTO
    {
        public int ProveedorId { get; set; }

        public string NombreEncargado { get; set; } = null!;

        public string Empresa { get; set; } = null!;

        public string? Telefono { get; set; }

        public string? ImagenUrl { get; set; }

        public bool Estado { get; set; }

        public int CatProveedorId { get; set; }

        public string? NombreCategoria { get; set; } // Para mostrar el nombre de la categoría del proveedor
    }
}
