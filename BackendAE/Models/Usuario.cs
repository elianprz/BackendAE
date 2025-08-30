using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UsuarioId { get; set; }

        [Required]
        [StringLength(60)]
        public required string PrimerNombre { get; set; }

        [StringLength(60)]
        public string? SegundoNombre { get; set; }

        [Required]
        [StringLength(60)]
        public required string PrimerApellido { get; set; }

        [StringLength(60)]
        public string? SegundoApellido { get; set; }

        [StringLength(20)]
        public string? NIT { get; set; }

        [StringLength(25)]
        public string? CUI { get; set; }

        [Required]
        public required DateTime FechaIngreso { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        [StringLength(1)]
        public string? Genero { get; set; }

        [Required]
        public required bool Estado { get; set; }

        [Required]
        [StringLength(120)]
        public required string Email { get; set; }

        [Required]
        [StringLength(50)]
        public required string NombreUsuario { get; set; }

        [Required]
        [StringLength(255)]
        public required string PasswordHash { get; set; }

        // Tiempo de contraseña
        public DateTime FechaUltimoCambioContrasena { get; set; }

        // Clave foránea para la relación N:1 con Roles
        [ForeignKey("Rol")]
        public int RolId { get; set; }
        public Rol? Rol { get; set; }

        // Relaciones 1:N con otras tablas
        public ICollection<CajaSesion>? SesionesAbiertas { get; set; }
        public ICollection<CajaSesion>? SesionesCerradas { get; set; }
        public ICollection<MovimientoCaja>? MovimientosCaja { get; set; }
        public ICollection<Venta>? Ventas { get; set; }
        
    }
}