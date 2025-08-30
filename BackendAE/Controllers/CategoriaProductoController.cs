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
    public class CategoriaProductoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoriaProductoController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/CategoriaProducto
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaProductoDTO>>> GetCategorias()
        {
            var categorias = await _context.CategoriasProductos.ToListAsync();
            return Ok(_mapper.Map<List<CategoriaProductoDTO>>(categorias));
        }

        // GET: api/CategoriaProducto/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoriaProductoDTO>> GetCategoria(int id)
        {
            var categoria = await _context.CategoriasProductos.FindAsync(id);
            if (categoria == null) return NotFound();

            return _mapper.Map<CategoriaProductoDTO>(categoria);
        }

        // POST: api/CategoriaProducto
       [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        public async Task<ActionResult> CrearCategoria([FromBody] CategoriaProductoCreacionDTO dto)
        {
            var categoria = _mapper.Map<CategoriaProducto>(dto);

            _context.CategoriasProductos.Add(categoria);
            await _context.SaveChangesAsync();

            var dtoCreado = _mapper.Map<CategoriaProductoDTO>(categoria);
            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.CategoriaId }, dtoCreado);
        }

        // PUT: api/CategoriaProducto/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarCategoria(int id, [FromBody] CategoriaProductoCreacionDTO dto)
        {
            var categoria = await _context.CategoriasProductos.FindAsync(id);
            if (categoria == null) return NotFound();

            _mapper.Map(dto, categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CategoriaProducto/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarCategoria(int id)
        {
            var categoria = await _context.CategoriasProductos.FindAsync(id);
            if (categoria == null) return NotFound();

            _context.CategoriasProductos.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
