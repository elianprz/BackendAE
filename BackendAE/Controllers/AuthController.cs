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
                PrimerNombre = "Leidy",
                SegundoNombre = "Lilien",
                PrimerApellido = "Rosales",
                SegundoApellido = "Tozc",
                Email = "leidyrosales672@gmail.com",
                NombreUsuario = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(contrasenaTemporal),
                Estado = true,
                Telefono = "58670527",
                Direccion = "Pajachel, Sololá",
                NIT = "2457443200710",
                CUI = "2457443200710",
                FechaIngreso = DateTime.UtcNow,
                FechaNacimiento = DateTime.Parse("10-12-1975"),
                Genero = "F",
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
            //await _emailService.SendEmailAsync(admin.Email, "Bienvenido a nuestro sistema Admin", templatePath, replacements);
             _emailService.SendEmailAsync(admin.Email, "Bienvenido a nuestro sistema Admin", templatePath, replacements);

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
            // 1️⃣ Validar el modelo
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 2️⃣ Buscar al usuario (incluimos su rol)
            var user = await _context.Usuarios
                .Include(u => u.Rol)
                .SingleOrDefaultAsync(u => u.NombreUsuario == request.Username);

            // 3️⃣ Checar existencia y contraseña
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Credenciales inválidas.");

            // 4️⃣ Checar si el usuario está **habilitado** (Estado = true)
            if (!user.Estado)
                return Unauthorized("El usuario está deshabilitado.");

            // 5️⃣ Caducidad de contraseña (opcional)
            var diasDeCaducidad = 90;          // o cargable desde appsettings
            if (user.FechaUltimoCambioContrasena.AddDays(diasDeCaducidad) < DateTime.UtcNow)
                return Forbid("La contraseña ha caducado. Cámbiala para continuar.");

            // 6️⃣ Generar el JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.UsuarioId.ToString()),
        new Claim(ClaimTypes.Name, user.NombreUsuario),
        new Claim(ClaimTypes.Role, user.Rol?.RolNombre ?? "sinRol"),
        new Claim("Estado", user.Estado.ToString())   // <-- nuestro claim extra
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),         // 30 min de validez (ajusta si quieres refrescos)
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // 7️⃣ Responder con el token (y opcionalmente datos de usuario)
            return Ok(new
            {
                token = tokenString,
                usuarioId = user.UsuarioId,
                username = user.NombreUsuario,
                role = user.Rol?.RolNombre,
                status = user.Estado  // útil para el front, pero el claim se encarga también
            });
        }


        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}