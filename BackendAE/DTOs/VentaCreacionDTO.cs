namespace BackendAE.DTOs
{
    //public class VentaCreacionDTO
    //{
    //    public int ProductoId { get; set; }
    //    public int CantidadVendida { get; set; }
    //    public decimal PrecioUnitario { get; set; }   // precio al que se vende ese artículo
    //    public decimal EfectivoRecibido { get; set; }
    //    public decimal Total {  get; set; }
    //    public decimal Cambio { get; set; }
    //    public string EstadoVenta { get; set; } = default!;
    //    public int? CajaSesionId { get; set; }
    //}
    public class VentaCreacionDTO
    {
        // Campos de LÍNEA
        public int ProductoId { get; set; }
        public int CantidadVendida { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Campos de ENCABEZADO (Se repiten, innecesario en el DTO de línea)
        public decimal EfectivoRecibido { get; set; }
        public decimal Total { get; set; }
        public decimal Cambio { get; set; }
        public string EstadoVenta { get; set; } = default!;
        public int? CajaSesionId { get; set; }
    }
}
