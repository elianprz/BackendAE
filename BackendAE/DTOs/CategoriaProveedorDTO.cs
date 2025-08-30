namespace BackendAE.DTOs
{
    public class CategoriaProveedorDTO
    {
        public int CatProveedorId { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public bool Estado { get; set; }
    }
}
