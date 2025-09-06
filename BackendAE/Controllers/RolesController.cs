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
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RolesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Roles
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolDTO>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return Ok(_mapper.Map<List<RolDTO>>(roles));
        }

        // GET: api/Roles/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RolDTO>> GetRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return NotFound();
            return Ok(_mapper.Map<RolDTO>(rol));
        }

        // POST: api/Roles
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CrearRol([FromBody] RolDTOCrear dto)
        {
            //var rol = _mapper.Map<Rol>(dto);
            //_context.Roles.Add(rol);
            //await _context.SaveChangesAsync();

            //var rolDTO = _mapper.Map<RolDTO>(rol);
            //return CreatedAtAction(nameof(GetRol), new { id = rol.RolId }, rolDTO);
            var rol = _mapper.Map<Rol>(dto);  // convierte el DTO a la entidad
            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();

            var rolDTO = _mapper.Map<RolDTO>(rol); // opcional, para devolver al cliente
            return Ok(rolDTO);
        }

        // PUT: api/Roles/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarRol(int id, [FromBody] RolDTO dto)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return NotFound();

            _mapper.Map(dto, rol);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Roles/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarRol(int id)
        {
            var rol = await _context.Roles.FindAsync(id);
            if (rol == null) return NotFound();

            _context.Roles.Remove(rol);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
