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
    public class ProveedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

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
            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            var proveedorDTO = _mapper.Map<ProveedorDTO>(proveedor);
            return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.ProveedorId }, proveedorDTO);
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
            if (proveedor == null) return NotFound();

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
