namespace BackendAE.DTOs
{
    public class VentaLoteCreacionDTO // Lo que envía el frontend
    {
        public required decimal Total { get; set; }
        public required decimal EfectivoRecibido { get; set; }
        public required decimal Cambio { get; set; }
        public required string EstadoVenta { get; set; } = "Finalizada"; // Ejemplo
        public int? CajaSesionId { get; set; }

        // Lista de productos. Usa un DTO simple para el detalle.
        public required List<VentaDetalleCreacionDTO> Items { get; set; }
    }
}
