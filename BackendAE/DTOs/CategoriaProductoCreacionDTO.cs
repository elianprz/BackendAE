namespace BackendAE.DTOs
{
    public class CategoriaProductoCreacionDTO
    {
        public required string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public bool Estado { get; set; }
    }
}
