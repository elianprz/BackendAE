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
    public class MovimientoCajasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MovimientoCajasController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/MovimientoCajas
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimientoCajaDTO>>> GetMovimientoCajas()
        {
            var movimientos = await _context.MovimientosCaja
                .Include(m => m.CajaSesion)
                .Include(m => m.Usuario)
                .ToListAsync();

            return Ok(_mapper.Map<List<MovimientoCajaDTO>>(movimientos));
        }

        // GET: api/MovimientoCajas/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovimientoCajaDTO>> GetMovimientoCaja(int id)
        {
            var movimiento = await _context.MovimientosCaja
                .Include(m => m.CajaSesion)
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.MovimientoCajaId == id);

            if (movimiento == null) return NotFound();

            return _mapper.Map<MovimientoCajaDTO>(movimiento);
        }

        // POST: api/MovimientoCajas
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CrearMovimientoCaja([FromBody] MovimientoCajaCreacionDTO dto)
        {
            var movimiento = _mapper.Map<MovimientoCaja>(dto);

            _context.MovimientosCaja.Add(movimiento);
            await _context.SaveChangesAsync();

            var dtoCreado = _mapper.Map<MovimientoCajaDTO>(movimiento);

            return CreatedAtAction(nameof(GetMovimientoCaja), new { id = movimiento.MovimientoCajaId }, dtoCreado);
        }

        // PUT: api/MovimientoCajas/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarMovimientoCaja(int id, [FromBody] MovimientoCajaCreacionDTO dto)
        {
            var movimiento = await _context.MovimientosCaja.FindAsync(id);

            if (movimiento == null) return NotFound();

            _mapper.Map(dto, movimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/MovimientoCajas/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarMovimientoCaja(int id)
        {
            var movimiento = await _context.MovimientosCaja.FindAsync(id);

            if (movimiento == null) return NotFound();

            _context.MovimientosCaja.Remove(movimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
