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
    public class CajaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CajaController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Caja
        [HttpGet]
       [Authorize(Roles = "Admin,Empleado")]
        public async Task<ActionResult<IEnumerable<CajaDTO>>> GetCajas()
        {
            var cajas = await _context.Cajas.ToListAsync();
            return Ok(_mapper.Map<List<CajaDTO>>(cajas));
        }

        // GET: api/Caja/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CajaDTO>> GetCaja(int id)
        {
            var caja = await _context.Cajas.FindAsync(id);
            if (caja == null) return NotFound();

            return _mapper.Map<CajaDTO>(caja);
        }

        // POST: api/Caja
       [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        public async Task<ActionResult> CrearCaja([FromBody] CajaCreacionDTO dto)
        {
            var caja = _mapper.Map<Caja>(dto);

            _context.Cajas.Add(caja);
            await _context.SaveChangesAsync();

            var dtoCreado = _mapper.Map<CajaDTO>(caja);
            return CreatedAtAction(nameof(GetCaja), new { id = caja.CajaId }, dtoCreado);
        }

        // PUT: api/Caja/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarCaja(int id, [FromBody] CajaCreacionDTO dto)
        {
            var caja = await _context.Cajas.FindAsync(id);
            if (caja == null) return NotFound();

            _mapper.Map(dto, caja);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Caja/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarCaja(int id)
        {
            var caja = await _context.Cajas.FindAsync(id);
            if (caja == null) return NotFound();

            _context.Cajas.Remove(caja);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
