namespace BackendAE.DTOs
{
    public class VentaDetalleCreacionDTO
    {
        public required int ProductoId { get; set; }
        public required int CantidadVendida { get; set; }
        public required decimal PrecioUnitario { get; set; }
    }
}