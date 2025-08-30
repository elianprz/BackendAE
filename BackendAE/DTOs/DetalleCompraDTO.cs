namespace BackendAE.DTOs
{
    public class DetalleCompraDTO
    {
        public int DetalleCompraId { get; set; }

        public int ProductoId { get; set; }

        public string? NombreProducto { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}
