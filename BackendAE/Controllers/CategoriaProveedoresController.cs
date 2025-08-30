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
    public class CategoriaProveedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoriaProveedoresController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/CategoriaProveedores
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaProveedorDTO>>> GetCategorias()
        {
            var categorias = await _context.CategoriasProveedores.ToListAsync();
            return Ok(_mapper.Map<List<CategoriaProveedorDTO>>(categorias));
        }

        // GET: api/CategoriaProveedores/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoriaProveedorDTO>> GetCategoria(int id)
        {
            var categoria = await _context.CategoriasProveedores.FindAsync(id);
            if (categoria == null) return NotFound();

            return _mapper.Map<CategoriaProveedorDTO>(categoria);
        }

        // POST: api/CategoriaProveedores
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CrearCategoria([FromBody] CategoriaProveedorCreacionDTO dto)
        {
            var categoria = _mapper.Map<CategoriaProveedor>(dto);
            _context.CategoriasProveedores.Add(categoria);
            await _context.SaveChangesAsync();

            var dtoCreado = _mapper.Map<CategoriaProveedorDTO>(categoria);
            return CreatedAtAction(nameof(GetCategoria), new { id = categoria.CatProveedorId }, dtoCreado);
        }

        // PUT: api/CategoriaProveedores/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarCategoria(int id, [FromBody] CategoriaProveedorCreacionDTO dto)
        {
            var categoria = await _context.CategoriasProveedores.FindAsync(id);
            if (categoria == null) return NotFound();

            _mapper.Map(dto, categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CategoriaProveedores/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarCategoria(int id)
        {
            var categoria = await _context.CategoriasProveedores.FindAsync(id);
            if (categoria == null) return NotFound();

            _context.CategoriasProveedores.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
