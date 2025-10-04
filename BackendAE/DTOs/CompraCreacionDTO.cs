namespace BackendAE.DTOs
{
    public class CompraCreacionDTO
    {
        // No enviamos CompraId ni Total (se calculan en el service)
        public DateTime? FechaCompra { get; set; }   // opcional, si no se envía se usará DateTime.UtcNow
        public string? Observacion { get; set; }

        // Información del producto (snapshot)
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public required bool Estado { get; set; } = true;
        public required int Stock { get; set; }                 // cantidad adquirida
        public required decimal PrecioAdquisicion { get; set; }
        public decimal? PrecioVenta { get; set; }

        // Proveedor
        public required int ProveedorId { get; set; }
    }
}
