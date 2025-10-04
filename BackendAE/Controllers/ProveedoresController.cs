using AutoMapper;
using BackendAE.Data;
using BackendAE.DTOs;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// 🔑 Importaciones para manejo de imágenes
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BackendAE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        // 🔑 Definimos la resolución máxima de la imagen para proveedores
        private const int MaxImageDimension = 5000;

        public ProveedoresController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Proveedores
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProveedorDTO>>> GetProveedores()
        {
            var proveedores = await _context.Proveedores
                .Include(p => p.CategoriaProveedor)
                .ToListAsync();

            var proveedoresDTO = _mapper.Map<List<ProveedorDTO>>(proveedores);
            return Ok(proveedoresDTO);
        }

        // GET: api/Proveedores/5
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProveedorDTO>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores
                .Include(p => p.CategoriaProveedor)
                .FirstOrDefaultAsync(p => p.ProveedorId == id);

            if (proveedor == null) return NotFound();

            return _mapper.Map<ProveedorDTO>(proveedor);
        }

        // POST: api/Proveedores
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CrearProveedor([FromBody] ProveedorCreacionDTO dto)
        {
            var proveedor = _mapper.Map<Proveedor>(dto);

            // ❌ Lógica de generación de SKU eliminada, ya que no aplica a Proveedores.

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            var proveedorDTO = _mapper.Map<ProveedorDTO>(proveedor);
            return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.ProveedorId }, proveedorDTO);
        }

        // 🆕 Endpoint para actualizar la imagen de un proveedor existente
        // PUT: api/Proveedores/ActualizarImagen/5
        [HttpPut("ActualizarImagen/{id:int}")]
        public async Task<ActionResult<string>> ActualizarImagenProveedor(int id, IFormFile image)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            if (image == null || image.Length == 0)
            {
                return BadRequest("No se ha seleccionado ninguna imagen.");
            }

            // Opcional: Eliminar la imagen antigua del servidor si existe
            if (!string.IsNullOrEmpty(proveedor.ImagenUrl))
            {
                var nombreArchivoAntiguo = Path.GetFileName(proveedor.ImagenUrl);
                var rutaAntigua = Path.Combine("wwwroot", "imagenes", nombreArchivoAntiguo);
                if (System.IO.File.Exists(rutaAntigua))
                {
                    System.IO.File.Delete(rutaAntigua);
                }
            }

            // Procesa y guarda la nueva imagen
            var rutaDirectorio = Path.Combine("wwwroot", "imagenes");
            var nombreArchivoNuevo = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var rutaCompletaNueva = Path.Combine(rutaDirectorio, nombreArchivoNuevo);

            using (var imagen = Image.Load(image.OpenReadStream()))
            {
                // ✅ Redimensiona la imagen a 5000x5000 píxeles, según tu requisito
                imagen.Mutate(x => x.Resize(MaxImageDimension, MaxImageDimension));
                await imagen.SaveAsync(rutaCompletaNueva);
            }

            // Actualiza la URL de la imagen en el proveedor
            var nuevaImagenUrl = $"{Request.Scheme}://{Request.Host}/imagenes/{nombreArchivoNuevo}";
            proveedor.ImagenUrl = nuevaImagenUrl;

            _context.Proveedores.Update(proveedor);
            await _context.SaveChangesAsync();

            // Devuelve la nueva URL en la respuesta
            return Ok(new { url = nuevaImagenUrl });
        }

        // 🆕 Endpoint para subir una imagen temporal (usado generalmente en la creación)
        // POST: api/Proveedores/SubirImagen
        [HttpPost("SubirImagen")]
        public async Task<IActionResult> SubirImagen(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No se ha seleccionado ninguna imagen.");
            }

            var rutaDirectorio = Path.Combine("wwwroot", "imagenes");
            if (!Directory.Exists(rutaDirectorio))
            {
                Directory.CreateDirectory(rutaDirectorio);
            }

            var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var rutaCompleta = Path.Combine(rutaDirectorio, nombreArchivo);

            try
            {
                using (var imagen = Image.Load(image.OpenReadStream()))
                {
                    // ✅ Redimensiona la imagen a 5000x5000 píxeles
                    imagen.Mutate(x => x.Resize(MaxImageDimension, MaxImageDimension));
                    await imagen.SaveAsync(rutaCompleta);
                }

                var urlImagen = $"{Request.Scheme}://{Request.Host}/imagenes/{nombreArchivo}";
                return Ok(new { url = urlImagen });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al procesar la imagen: {ex.Message}");
            }
        }


        // PUT: api/Proveedores/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarProveedor(int id, [FromBody] ProveedorCreacionDTO dto)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            _mapper.Map(dto, proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Proveedores/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            // Opcional: Puedes añadir lógica para eliminar la imagen asociada aquí también,
            // similar a la que se usa en ActualizarImagenProveedor.

            if (proveedor == null) return NotFound();

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}