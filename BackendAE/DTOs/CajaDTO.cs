namespace BackendAE.DTOs
{
    public class CajaDTO
    {
        public int CajaId { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public bool Estado { get; set; }
    }
}
