using AutoMapper;
using BackendAE.Data;
using BackendAE.DTOs;
using BackendAE.Models;
using BackendAE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;

namespace BackendAE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly EmailService _emailService; // Inyectar el servicio de email


        public UsuariosController(ApplicationDbContext context, IMapper mapper, EmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService; // Se asigna la instancia inyectada
        }

        // GET: api/Usuarios
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .ToListAsync();

            var usuariosDTO = _mapper.Map<List<UsuarioDTO>>(usuarios);
            return Ok(usuariosDTO);
        }

        // GET: api/Usuarios/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null) return NotFound();

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        // POST: api/Usuarios
        //[HttpPost]
        //public async Task<ActionResult> CrearUsuario([FromBody] UsuarioCreacionDTO dto)
        //{
        //    var usuario = _mapper.Map<Usuario>(dto);
        //    usuario.PasswordHash = dto.Contrasena; // Aquí deberías aplicar hashing

        //    _context.Usuarios.Add(usuario);
        //    await _context.SaveChangesAsync();

        //    var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);
        //    return CreatedAtAction(nameof(GetUsuario), new { id = usuario.UsuarioId }, usuarioDTO);
        //}
        // POST: api/Usuarios
        //[Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CrearUsuario([FromBody] UsuarioCreacionDTO dto)
        {
            // Verificación de campos obligatorios
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validar si el RolId existe
            var rolExistente = await _context.Roles.FindAsync(dto.RolId);
            if (rolExistente == null)
            {
                return BadRequest("El RolId especificado no existe.");
            }

            // 1. Mapear DTO a la entidad de Usuario
            //var usuario = _mapper.Map<Usuario>(dto);
            var nombreGenerado = $"{dto.PrimerNombre.ToLower().Substring(0, Math.Min(dto.PrimerNombre.Length, 3))}" +
                         $"{dto.PrimerApellido.ToLower().Substring(0, Math.Min(dto.PrimerApellido.Length, 3))}";
            
            var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == nombreGenerado);
            if (usuarioExistente != null)
            {
                // Puedes agregar una lógica para hacerlo único, por ejemplo, con un número.
                // Para este ejemplo, solo devolvemos un error.
                return BadRequest("El nombre de usuario generado ya existe. Por favor, intente con otro nombre o apellido.");
            }

            var usuario = _mapper.Map<Usuario>(dto);

            // 2. Generar y cifrar la contraseña
            usuario.NombreUsuario = nombreGenerado;
            var contrasenaTemporal = Guid.NewGuid().ToString().Substring(0, 8);
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(contrasenaTemporal);

            // 3. Asignar FechaUltimoCambioContrasena
            usuario.FechaUltimoCambioContrasena = DateTime.UtcNow;

            // 4. Agregar el usuario al contexto
            _context.Usuarios.Add(usuario);

            try
            {
                // 5. Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();

                // 6. Enviar el email solo si el guardado fue exitoso
                //var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "Bienvenida.html");
                //var replacements = new Dictionary<string, string>
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "BienvenidaUsuario.html");
                var replacements = new Dictionary<string, string>
        {
            { "@PrimerNombre", usuario.PrimerNombre },
            { "@NombreUsuario", usuario.NombreUsuario },
            { "@ContrasenaTemporal", contrasenaTemporal }
        };
                await _emailService.SendEmailAsync(usuario.Email, "Bienvenido a nuestro sistema", templatePath, replacements);
            }
            catch (DbUpdateException ex)
            {
                // Puedes ver los detalles de la excepción interna para depurar
                var innerExceptionMessage = ex.InnerException?.Message;
                // Devuelve un error genérico en producción para no exponer detalles sensibles
                return StatusCode(500, $"Ocurrió un error al guardar el usuario: {innerExceptionMessage}");
            }

            var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.UsuarioId }, usuarioDTO);
        }

        //private string GenerateRandomPassword()
        //{
        //    return Guid.NewGuid().ToString().Substring(0, 8); // Genera una cadena aleatoria de 8 caracteres
        //}

        // PUT: api/Usuarios/5
        //[HttpPut("{id:int}")]
        //public async Task<ActionResult> ActualizarUsuario(int id, [FromBody] UsuarioCreacionDTO dto)
        //{
        //    var usuario = await _context.Usuarios.FindAsync(id);
        //    if (usuario == null) return NotFound();

        //    _mapper.Map(dto, usuario);
        //    usuario.PasswordHash = dto.Contrasena; // Aquí también debería aplicarse hashing

        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarUsuario(int id, [FromBody] UsuarioActualizacionDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            // Mapea solo los campos actualizables
            _mapper.Map(dto, usuario);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin,Empleado")]
        [HttpPatch("{id:int}/cambiar-contrasena")]
        public async Task<ActionResult> CambiarContrasena(int id, [FromBody] CambioContrasenaDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(dto.ContrasenaActual, usuario.PasswordHash))
            {
                return BadRequest("La contraseña actual es incorrecta.");
            }

            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NuevaContrasena);
            usuario.FechaUltimoCambioContrasena = DateTime.UtcNow; // <--- Agrega esta línea

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // UsuariosController.cs
        [HttpPost("recuperar-contrasena")]
        public async Task<ActionResult> RecuperarContrasena([FromBody] RecuperarContrasenaDTO dto)
        {
            // ... (Tu lógica para verificar el correo y actualizar la contraseña)
            if (string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest("El correo electrónico es requerido.");
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario == null)
            {
                return Ok("Si el correo electrónico está asociado a una cuenta, recibirás un correo con una nueva contraseña temporal.");
            }

            // Generar una nueva contraseña temporal
            var nuevaContrasenaTemporal = Guid.NewGuid().ToString().Substring(0, 8);

            // Cifrar y actualizar la contraseña del usuario
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaContrasenaTemporal);
            usuario.FechaUltimoCambioContrasena = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();

                // Si la actualización es exitosa, intenta enviar el correo.
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "RecuperarContrasena.html");
                var replacements = new Dictionary<string, string>
        {
            { "@PrimerNombre", usuario.PrimerNombre },
            { "@NombreUsuario", usuario.NombreUsuario },
            { "@ContrasenaTemporal", nuevaContrasenaTemporal }
        };

                await _emailService.SendEmailAsync(usuario.Email, "Recuperación de contraseña", templatePath, replacements);

                return Ok("Se ha enviado una nueva contraseña temporal a tu correo electrónico.");
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Ocurrió un error al intentar guardar la nueva contraseña.");
            }
            catch (Exception ex)
            {
                // En caso de que falle el envío del correo, puedes devolver un error o
                // simplemente no hacer nada y dejar que el frontend reciba la respuesta.
                // Aquí retornaremos un mensaje de éxito, ya que la contraseña ya se actualizó
                // en la base de datos.
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                return Ok("La contraseña ha sido actualizada, pero hubo un problema al enviar el correo de notificación. Por favor, revisa tu bandeja de entrada.");
            }
        }

        // DELETE: api/Usuarios/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
