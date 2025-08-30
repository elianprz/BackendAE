using BackendAE.Data;
using BackendAE.Models;
using BackendAE.Services;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendAE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public AuthController(ApplicationDbContext context, IConfiguration configuration, EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin()
        {
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.RolNombre == "Admin");
            if (adminRole == null)
            {
                adminRole = new Rol { RolNombre = "Admin" };
                _context.Roles.Add(adminRole);
                await _context.SaveChangesAsync();
            }

            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == "admin"))
                return BadRequest("El usuario admin ya existe.");

            var contrasenaTemporal = Guid.NewGuid().ToString().Substring(0, 8);

            var admin = new Usuario
            {
                PrimerNombre = "admin",
                SegundoNombre = "admin",
                PrimerApellido = "admin",
                SegundoApellido = "admin",
                Email = "elianp812@gmail.com",
                NombreUsuario = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(contrasenaTemporal),
                Estado = true,
                Telefono = "12345678",
                Direccion = "admin",
                NIT = "9000034567890",
                CUI = "9000034567890",
                FechaIngreso = DateTime.UtcNow,
                FechaNacimiento = DateTime.Parse("03-06-2002"),
                Genero = "M",
                FechaUltimoCambioContrasena = DateTime.UtcNow,
                RolId = adminRole.RolId
            };

            _context.Usuarios.Add(admin);
            await _context.SaveChangesAsync();

            // Enviar correo de bienvenida
            //var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "Bienvenida.html");
            //var replacements = new Dictionary<string, string>
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "BienvenidaAdmin.html");
            var replacements = new Dictionary<string, string>
    {
        { "@NombreUsuario", admin.NombreUsuario },
        { "@ContrasenaTemporal", contrasenaTemporal }
    };
            await _emailService.SendEmailAsync(admin.Email, "Bienvenido a nuestro sistema Admin", templatePath, replacements);

            return Ok(new
            {
                message = "Usuario admin creado correctamente.",
                usuario = admin.NombreUsuario,
                contrasenaTemporal
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Usuarios
                .Include(u => u.Rol)
                .SingleOrDefaultAsync(u => u.NombreUsuario == request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            // Lógica de caducidad
            var diasDeCaducidad = 90; // Puedes mover este valor a appsettings.json
            if (user.FechaUltimoCambioContrasena.AddDays(diasDeCaducidad) < DateTime.UtcNow)
            {
                // Devolver un código de estado específico o un mensaje claro
                return Forbid("La contraseña ha caducado. Por favor, cámbiela para continuar.");
            }
            // Generar el token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()),
                    new Claim(ClaimTypes.Name, user.NombreUsuario),
                    new Claim(ClaimTypes.Role, user.Rol?.RolNombre ?? "sin6_rol")
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}