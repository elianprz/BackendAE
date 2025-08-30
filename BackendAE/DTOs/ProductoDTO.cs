namespace BackendAE.DTOs
{
    public class ProductoDTO
    {
        public int ProductoId { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public bool Estado { get; set; }

        public int Stock { get; set; }

        public decimal PrecioAdquisicion { get; set; }

        public decimal PrecioVenta { get; set; }

        public string? ImagenUrl { get; set; }

        public string? SKU { get; set; }

        public int CategoriaId { get; set; }

        public string? NombreCategoria { get; set; } // Para mostrar el nombre de la categoría
    }
}
