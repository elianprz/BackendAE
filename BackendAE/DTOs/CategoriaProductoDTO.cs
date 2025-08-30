namespace BackendAE.DTOs
{
    public class CategoriaProductoDTO
    {
        public int CategoriaId { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public bool Estado { get; set; }
    }
}
