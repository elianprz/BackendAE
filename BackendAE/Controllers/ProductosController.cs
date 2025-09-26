using AutoMapper;
using BackendAE.Data;
using BackendAE.DTOs;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// Img
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BackendAE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Productos
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductos()
        {
            var productos = await _context.Productos
                .Include(p => p.CategoriaProducto)
                .ToListAsync();

            var productosDTO = _mapper.Map<List<ProductoDTO>>(productos);
            return Ok(productosDTO);
        }

        // GET: api/Productos/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoDTO>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.CategoriaProducto)
                .FirstOrDefaultAsync(p => p.ProductoId == id);

            if (producto == null) return NotFound();

            return _mapper.Map<ProductoDTO>(producto);
        }


        //[HttpPost]
        //public async Task<ActionResult> CrearProducto([FromBody] ProductoCreacionDTO dto)
        //{
        //    var producto = _mapper.Map<Producto>(dto);

        //    _context.Productos.Add(producto);
        //    await _context.SaveChangesAsync();

        //    var productoDTO = _mapper.Map<ProductoDTO>(producto);
        //    return CreatedAtAction(nameof(GetProducto), new { id = producto.ProductoId }, productoDTO);
        //}
        // POST: api/Productos
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CrearProducto([FromBody] ProductoCreacionDTO dto)
        {
            // Mapea el DTO a la entidad Producto
            var producto = _mapper.Map<Producto>(dto);

            // ✅ Lógica para generar el SKU
            var ultimoProducto = await _context.Productos
                .OrderByDescending(p => p.ProductoId)
                .FirstOrDefaultAsync();

            int ultimoNumero = 0;
            string sufijo = "AL"; // Sufijo estático o puedes generar uno dinámicamente

            if (ultimoProducto != null && !string.IsNullOrEmpty(ultimoProducto.SKU))
            {
                // Extrae el número del último SKU (ej. "sku-0001AL" -> 1)
                var partes = ultimoProducto.SKU.Split('-');
                if (partes.Length >= 2 && partes[1].Length >= 4 && int.TryParse(partes[1].Substring(0, 4), out int numero))
                {
                    ultimoNumero = numero;
                }
            }

            int nuevoNumero = ultimoNumero + 1;
            string nuevoNumeroFormateado = nuevoNumero.ToString("D4"); // Formato a 4 dígitos, ej. "0002"

            producto.SKU = $"sku-{nuevoNumeroFormateado}{sufijo}";

            // Guarda el producto en la base de datos
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Mapea el producto con el nuevo SKU a un DTO y lo devuelve
            var productoDTO = _mapper.Map<ProductoDTO>(producto);
            return CreatedAtAction(nameof(GetProducto), new { id = producto.ProductoId }, productoDTO);
        }

        [HttpPut("ActualizarImagen/{id:int}")]
        public async Task<ActionResult<string>> ActualizarImagenProducto(int id, IFormFile image)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            if (image == null || image.Length == 0)
            {
                return BadRequest("No se ha seleccionado ninguna imagen.");
            }

            // Opcional: Eliminar la imagen antigua del servidor si existe
            if (!string.IsNullOrEmpty(producto.ImagenUrl))
            {
                var nombreArchivoAntiguo = Path.GetFileName(producto.ImagenUrl);
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
                imagen.Mutate(x => x.Resize(1000, 1000));
                await imagen.SaveAsync(rutaCompletaNueva);
            }

            // Actualiza la URL de la imagen en el producto
            var nuevaImagenUrl = $"{Request.Scheme}://{Request.Host}/imagenes/{nombreArchivoNuevo}";
            producto.ImagenUrl = nuevaImagenUrl;

            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();

            // ✅ CORRECCIÓN CLAVE: Devuelve la nueva URL en la respuesta
            return Ok(new { url = nuevaImagenUrl });
        }

        // PUT: api/Productos/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarProducto(int id, [FromBody] ProductoCreacionDTO dto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            _mapper.Map(dto, producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Subir img
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
                    // ✅ Redimensiona la imagen a 1000x1000 píxeles
                    imagen.Mutate(x => x.Resize(1000, 1000));
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

        // DELETE: api/Productos/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
