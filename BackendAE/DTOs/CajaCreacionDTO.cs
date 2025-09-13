namespace BackendAE.DTOs
{
    public class CajaCreacionDTO
    {
        public required string Nombre { get; set; }

        public string? Descripcion { get; set; }

        public bool Estado { get; set; }
    }
}
