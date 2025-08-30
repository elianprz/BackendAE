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
    public class ComprasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ComprasController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Compras
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraDTO>>> GetCompras()
        {
            var compras = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.DetalleCompras!)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();

            return Ok(_mapper.Map<List<CompraDTO>>(compras));
        }

        // GET: api/Compras/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CompraDTO>> GetCompra(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.DetalleCompras!)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(c => c.CompraId == id);

            if (compra == null) return NotFound();

            return _mapper.Map<CompraDTO>(compra);
        }

        // POST: api/Compras
       [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        public async Task<ActionResult> CrearCompra([FromBody] CompraCreacionDTO dto)
        {
            var compra = _mapper.Map<Compra>(dto);

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            var compraDTO = _mapper.Map<CompraDTO>(compra);
            return CreatedAtAction(nameof(GetCompra), new { id = compra.CompraId }, compraDTO);
        }

        // PUT: api/Compras/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarCompra(int id, [FromBody] CompraDTO dto)
        {
            if (id != dto.CompraId) return BadRequest("El ID de la compra no coincide.");
            var compra = await _context.Compras
                .Include(c => c.DetalleCompras)
                .FirstOrDefaultAsync(c => c.CompraId == id);
            if (compra == null) return NotFound();
            _mapper.Map(dto, compra);
            _context.Entry(compra).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Compras/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarCompra(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.DetalleCompras)
                .FirstOrDefaultAsync(c => c.CompraId == id);

            if (compra == null) return NotFound();

            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
