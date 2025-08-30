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
    public class DetalleComprasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DetalleComprasController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/DetalleCompras
        //[Authorize(Roles = "Admin")]
        //[Authorize(Policy = "Empleado")]
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetalleCompraDTO>>> GetDetalleCompras()
        {
            var detalles = await _context.DetalleCompras
                .Include(dc => dc.Compra)
                .Include(dc => dc.Producto)
                .ToListAsync();

            return Ok(_mapper.Map<List<DetalleCompraDTO>>(detalles));
        }

        // GET: api/DetalleCompras/5
       [Authorize(Roles = "Admin,Empleado")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DetalleCompraDTO>> GetDetalleCompra(int id)
        {
            var detalle = await _context.DetalleCompras
                .Include(dc => dc.Compra)
                .Include(dc => dc.Producto)
                .FirstOrDefaultAsync(dc => dc.DetalleCompraId == id);

            if (detalle == null) return NotFound();

            return _mapper.Map<DetalleCompraDTO>(detalle);
        }

        //// POST: api/DetalleCompras
        //[HttpPost]
        //public async Task<ActionResult> CrearDetalleCompra([FromBody] DetalleCompraCreacionDTO dto)
        //{
        //    var detalle = _mapper.Map<DetalleCompra>(dto);

        //    _context.DetalleCompras.Add(detalle);
        //    await _context.SaveChangesAsync();

        //    var dtoCreado = _mapper.Map<DetalleCompraDTO>(detalle);

        //    return CreatedAtAction(nameof(GetDetalleCompra), new { id = detalle.DetalleCompraId }, dtoCreado);
        //}

        // PUT: api/DetalleCompras/5
        //[HttpPut("{id:int}")]
        //public async Task<ActionResult> ActualizarDetalleCompra(int id, [FromBody] DetalleCompraCreacionDTO dto)
        //{
        //    var detalle = await _context.DetalleCompras.FindAsync(id);

        //    if (detalle == null) return NotFound();

        //    _mapper.Map(dto, detalle);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        // DELETE: api/DetalleCompras/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarDetalleCompra(int id)
        {
            var detalle = await _context.DetalleCompras.FindAsync(id);

            if (detalle == null) return NotFound();

            _context.DetalleCompras.Remove(detalle);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
