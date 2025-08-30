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
    public class CajaSesionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CajaSesionesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/CajaSesiones
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CajaSesionDTO>>> GetCajaSesiones()
        {
            var sesiones = await _context.CajaSesiones
                .Include(c => c.Caja)
                .Include(c => c.UsuarioApertura)
                .Include(c => c.UsuarioCierre)
                .ToListAsync();

            return Ok(_mapper.Map<List<CajaSesionDTO>>(sesiones));
        }

        // GET: api/CajaSesiones/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CajaSesionDTO>> GetCajaSesion(int id)
        {
            var sesion = await _context.CajaSesiones
                .Include(c => c.Caja)
                .Include(c => c.UsuarioApertura)
                .Include(c => c.UsuarioCierre)
                .FirstOrDefaultAsync(c => c.CajaSesionId == id);

            if (sesion == null) return NotFound();

            return _mapper.Map<CajaSesionDTO>(sesion);
        }

        // POST: api/CajaSesiones
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CrearCajaSesion([FromBody] CajaSesionCreacionDTO dto)
        {
            var sesion = _mapper.Map<CajaSesion>(dto);

            _context.CajaSesiones.Add(sesion);
            await _context.SaveChangesAsync();

            var dtoCreado = _mapper.Map<CajaSesionDTO>(sesion);

            return CreatedAtAction(nameof(GetCajaSesion), new { id = sesion.CajaSesionId }, dtoCreado);
        }

        // PUT: api/CajaSesiones/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarCajaSesion(int id, [FromBody] CajaSesionCreacionDTO dto)
        {
            var sesion = await _context.CajaSesiones.FindAsync(id);

            if (sesion == null) return NotFound();

            _mapper.Map(dto, sesion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CajaSesiones/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarCajaSesion(int id)
        {
            var sesion = await _context.CajaSesiones.FindAsync(id);

            if (sesion == null) return NotFound();

            _context.CajaSesiones.Remove(sesion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
