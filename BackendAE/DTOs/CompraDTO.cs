namespace BackendAE.DTOs
{
    public class CompraDTO
    {
        public int CompraId { get; set; }
        public DateTime FechaCompra { get; set; }

        public decimal Total { get; set; }

        public string? Observacion { get; set; }

        public int ProveedorId { get; set; }

        public string? NombreEncargado { get; set; }

        public List<DetalleCompraDTO> DetalleCompras { get; set; } = new();
    }
}
