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

        // -------------------------------------------------
        //  GET: api/Compras
        // -------------------------------------------------
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompraDTO>>> GetCompras()
        {
            var compras = await _context.Compras
                .Include(c => c.Proveedor)               // sigue siendo útil para mostrar el proveedor
                .ToListAsync();

            return Ok(_mapper.Map<List<CompraDTO>>(compras));
        }

        // -------------------------------------------------
        //  GET: api/Compras/{id}
        // -------------------------------------------------
        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CompraDTO>> GetCompra(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.Proveedor)
                .FirstOrDefaultAsync(c => c.CompraId == id);

            if (compra == null) return NotFound();

            return Ok(_mapper.Map<CompraDTO>(compra));
        }

        // -------------------------------------------------
        //  POST: api/Compras
        // -------------------------------------------------
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        public async Task<ActionResult> CrearCompra([FromBody] CompraCreacionDTO dto)
        {
            // 1️⃣ Mapeamos al modelo entidad
            var compra = _mapper.Map<Compra>(dto);

            // 2️⃣ Campos que no vienen del cliente:
            compra.FechaCompra = dto.FechaCompra ?? DateTime.UtcNow;

            // 3️⃣ Calculamos el total de la compra (precio unidad × cantidad)
            compra.Total = compra.PrecioAdquisicion * compra.Stock;   // Stock = cantidad comprada

            // 4️⃣ Guardamos la compra (***NO*** modificamos el stock de Producto)
            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            var compraDto = _mapper.Map<CompraDTO>(compra);
            return CreatedAtAction(nameof(GetCompra), new { id = compra.CompraId }, compraDto);
        }

        // -------------------------------------------------
        //  PUT: api/Compras/{id}
        // -------------------------------------------------
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarCompra(int id, [FromBody] CompraDTO dto)
        {
            if (id != dto.CompraId) return BadRequest("El ID de la compra no coincide.");

            var compraBd = await _context.Compras.FirstOrDefaultAsync(c => c.CompraId == id);
            if (compraBd == null) return NotFound();

            // Mapear los cambios (excluye Id y Total, los recalculamos)
            _mapper.Map(dto, compraBd);
            compraBd.Total = compraBd.PrecioAdquisicion * compraBd.Stock; // recalcular

            // *** No tocamos el stock del producto ***
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // -------------------------------------------------
        //  DELETE: api/Compras/{id}
        // -------------------------------------------------
        [Authorize(Roles = "Admin,Empleado")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarCompra(int id)
        {
            var compra = await _context.Compras.FirstOrDefaultAsync(c => c.CompraId == id);
            if (compra == null) return NotFound();

            // *** No modificamos el stock del producto al borrar la compra ***
            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
