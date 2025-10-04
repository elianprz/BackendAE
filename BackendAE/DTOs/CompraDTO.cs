namespace BackendAE.DTOs
{
    public class CompraDTO
    {
        public int CompraId { get; set; }
        public DateTime FechaCompra { get; set; }
        public string? Observacion { get; set; }

        // Campos copiados del producto
        public string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
        public int Stock { get; set; }                     // cantidad comprada
        public decimal PrecioAdquisicion { get; set; }     // precio por unidad
        public decimal? PrecioVenta { get; set; }          // referencia de venta (opcional)

        // Relación
        public int ProveedorId { get; set; }
        public string? NombreProveedor { get; set; }      // opcional para el response

        // Total calculado (PrecioAdquisicion * Stock)
        public decimal Total { get; set; }
    }
}
