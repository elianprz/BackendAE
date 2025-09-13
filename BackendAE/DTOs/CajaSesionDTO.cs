namespace BackendAE.DTOs
{
    public class CajaSesionDTO
    {
        public int CajaSesionId { get; set; }
        
        public DateTime FechaApertura { get; set; }

        public decimal MontoApertura { get; set; }
        
        public DateTime? FechaCierre { get; set; }

        public decimal MontoCierre { get; set; }

        public bool Estado { get; set; }

        public string? Observacion { get; set; }

        public int CajaId { get; set; }

        public string? NombreCaja { get; set; }

        public int UsuarioAperturaId { get; set; }

        public int? UsuarioCierreId { get; set; }

        //public int? UsuarioId { get; set; }
    }
}
