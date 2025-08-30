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
    public class DetalleVentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DetalleVentasController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/DetalleVentas
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetalleVentaDTO>>> GetDetalleVentas()
        {
            var detalles = await _context.DetallesVentas
                .Include(dv => dv.Venta)
                .Include(dv => dv.Producto)
                .ToListAsync();

            return Ok(_mapper.Map<List<DetalleVentaDTO>>(detalles));
        }

        // GET: api/DetalleVentas/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DetalleVentaDTO>> GetDetalleVenta(int id)
        {
            var detalle = await _context.DetallesVentas
                .Include(dv => dv.Venta)
                .Include(dv => dv.Producto)
                .FirstOrDefaultAsync(dv => dv.DetalleVentaId == id);

            if (detalle == null) return NotFound();

            return _mapper.Map<DetalleVentaDTO>(detalle);
        }

        //// POST: api/DetalleVentas
        //[HttpPost]
        //public async Task<ActionResult> CrearDetalleVenta([FromBody] DetalleVentaCreacionDTO dto)
        //{
        //    var detalle = _mapper.Map<DetalleVenta>(dto);

        //    _context.DetallesVentas.Add(detalle);
        //    await _context.SaveChangesAsync();

        //    var dtoCreado = _mapper.Map<DetalleVentaDTO>(detalle);

        //    return CreatedAtAction(nameof(GetDetalleVenta), new { id = detalle.DetalleVentaId }, dtoCreado);
        //}

        // PUT: api/DetalleVentas/5
        //[HttpPut("{id:int}")]
        //public async Task<ActionResult> ActualizarDetalleVenta(int id, [FromBody] DetalleVentaCreacionDTO dto)
        //{
        //    var detalle = await _context.DetallesVentas.FindAsync(id);

        //    if (detalle == null) return NotFound();

        //    _mapper.Map(dto, detalle);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        // DELETE: api/DetalleVentas/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarDetalleVenta(int id)
        {
            var detalle = await _context.DetallesVentas.FindAsync(id);

            if (detalle == null) return NotFound();

            _context.DetallesVentas.Remove(detalle);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
