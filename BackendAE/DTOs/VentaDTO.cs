namespace BackendAE.DTOs
{
    public class VentaDTO
    {
        public int VentaId { get; set; }
        public DateTime FechaVenta { get; set; }
        public string CodigoVenta { get; set; } = default!;
        public decimal Total { get; set; }
        public decimal EfectivoRecibido { get; set; }
        public decimal Cambio { get; set; }
        public string EstadoVenta { get; set; } = default!;
        public int UsuarioId { get; set; }
        public int? CajaSesionId { get; set; }

        // Campos “planos” del producto vendido
        public int ProductoId { get; set; }
        public int CantidadVendida { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
    }
}
