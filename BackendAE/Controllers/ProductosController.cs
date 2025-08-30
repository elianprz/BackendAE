using AutoMapper;
using BackendAE.Data;
using BackendAE.DTOs;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // POST: api/Productos
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CrearProducto([FromBody] ProductoCreacionDTO dto)
        {
            var producto = _mapper.Map<Producto>(dto);

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var productoDTO = _mapper.Map<ProductoDTO>(producto);
            return CreatedAtAction(nameof(GetProducto), new { id = producto.ProductoId }, productoDTO);
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
