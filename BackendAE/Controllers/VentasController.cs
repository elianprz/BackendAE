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
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public VentasController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Ventas
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDTO>>> GetVentas()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.CajaSesion)
                .Include(v => v.DetalleVentas!)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();

            return Ok(_mapper.Map<List<VentaDTO>>(ventas));
        }

        // GET: api/Ventas/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VentaDTO>> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.CajaSesion)
                .Include(v => v.DetalleVentas!)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(v => v.VentaId == id);

            if (venta == null) return NotFound();

            return _mapper.Map<VentaDTO>(venta);
        }

        // POST: api/Ventas
       [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        public async Task<ActionResult> CrearVenta([FromBody] VentaCreacionDTO dto)
        {
            var venta = _mapper.Map<Venta>(dto);

            venta.CodigoVenta = "VTA-" + DateTime.Now.ToString("yyyyMMddHHmmss"); // Or generate it however you need.
            venta.FechaVenta = DateTime.Now;
            

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            var ventaDTO = _mapper.Map<VentaDTO>(venta);
            return CreatedAtAction(nameof(GetVenta), new { id = venta.VentaId }, ventaDTO);
        }

        // PUT: api/Ventas/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarVenta(int id, [FromBody] VentaDTO dto)
        {
            if (id != dto.VentaId) return BadRequest("El ID de la venta no coincide.");
            var venta = await _context.Ventas
                .Include(v => v.DetalleVentas)
                .FirstOrDefaultAsync(v => v.VentaId == id);
            if (venta == null) return NotFound();
            _mapper.Map(dto, venta);
            _context.Entry(venta).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Ventas/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.DetalleVentas)
                .FirstOrDefaultAsync(v => v.VentaId == id);

            if (venta == null) return NotFound();

            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
