using AutoMapper;
using BackendAE.Data;
using BackendAE.DTOs;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        //[Authorize(Roles = "Admin,Empleado")]
        //[HttpPost]
        //public async Task<ActionResult<VentaDTO>> PostVenta([FromBody] VentaCreacionDTO ventaDto)
        //{
        //    // 1. Generar el código de venta
        //    var ultimoCodigo = await _context.Ventas.OrderByDescending(v => v.VentaId).Select(v => v.CodigoVenta).FirstOrDefaultAsync();
        //    var codigoVenta = GenerarSiguienteCodigo(ultimoCodigo);


        //    var usuarioIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        //    if (usuarioIdClaim == null)
        //    {
        //        return BadRequest("UsuarioId no encontrado en el token.");
        //    }

        //    // Convertir el string del claim a int
        //    if (!int.TryParse(usuarioIdClaim.Value, out int usuarioId))
        //    {
        //        return BadRequest("Formato de UsuarioId inválido en el token.");
        //    }
        //    // 2. Mapear DTO a modelo y añadir los detalles
        //    var venta = new Venta
        //    {
        //        FechaVenta = DateTime.Now,
        //        CodigoVenta = codigoVenta,
        //        Total = ventaDto.Total,
        //        EfectivoRecibido = ventaDto.EfectivoRecibido,
        //        Cambio = ventaDto.Cambio,
        //        EstadoVenta = ventaDto.EstadoVenta,
        //        UsuarioId = usuarioId,
        //        CajaSesionId = ventaDto.CajaSesionId,
        //        // ✅ CORRECCIÓN CLAVE: Calcular y asignar el Subtotal para cada detalle
        //        DetalleVentas = ventaDto.DetalleVentas.Select(detalleDto => new DetalleVenta
        //        {
        //            ProductoId = detalleDto.ProductoId,
        //            Cantidad = detalleDto.Cantidad,
        //            PrecioUnitario = detalleDto.PrecioUnitario,
        //            // ✅ Aquí se calcula el subtotal para cada ítem
        //            Subtotal = detalleDto.Cantidad * detalleDto.PrecioUnitario
        //        }).ToList()
        //    };

        //    // 3. Añadir la venta al contexto
        //    _context.Ventas.Add(venta);

        //    // 4. Actualizar el MontoCierre de la sesión de caja
        //    if (ventaDto.CajaSesionId.HasValue)
        //    {
        //        var sesionCaja = await _context.CajaSesiones.FindAsync(ventaDto.CajaSesionId.Value);
        //        if (sesionCaja != null)
        //        {
        //            sesionCaja.MontoCierre += venta.Total;
        //        }
        //    }

        //    // 5. Actualizar el stock de los productos
        //    foreach (var detalle in venta.DetalleVentas)
        //    {
        //        var producto = await _context.Productos.FindAsync(detalle.ProductoId);
        //        if (producto != null)
        //        {
        //            producto.Stock -= detalle.Cantidad;
        //        }
        //    }

        //    // 6. Guardar los cambios en la base de datos
        //    await _context.SaveChangesAsync();

        //    // 7. Mapear el modelo de vuelta a un DTO para la respuesta y devolverlo
        //    var ventaDTO = _mapper.Map<VentaDTO>(venta);
        //    return CreatedAtAction(nameof(GetVenta), new { id = venta.VentaId }, ventaDTO);
        //}
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        public async Task<ActionResult<VentaDTO>> PostVenta([FromBody] VentaCreacionDTO ventaDto)
        {
            // 1. Generar el código de venta
            var ultimoCodigo = await _context.Ventas.OrderByDescending(v => v.VentaId).Select(v => v.CodigoVenta).FirstOrDefaultAsync();
            var codigoVenta = GenerarSiguienteCodigo(ultimoCodigo);

            var usuarioIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null || !int.TryParse(usuarioIdClaim.Value, out int usuarioId))
            {
                return BadRequest("Usuario no válido.");
            }

            // 2. Obtener la sesión de caja para su posterior actualización
            var sesionCaja = await _context.CajaSesiones.FindAsync(ventaDto.CajaSesionId.Value);
            if (sesionCaja == null)
            {
                return NotFound("Sesión de caja no encontrada.");
            }

            // 3. Preparar la entidad Venta
            var venta = new Venta
            {
                FechaVenta = DateTime.Now,
                CodigoVenta = codigoVenta,
                Total = ventaDto.Total,
                EfectivoRecibido = ventaDto.EfectivoRecibido,
                Cambio = ventaDto.Cambio,
                EstadoVenta = ventaDto.EstadoVenta,
                UsuarioId = usuarioId,
                CajaSesionId = ventaDto.CajaSesionId,
                DetalleVentas = new List<DetalleVenta>()
            };

            // 4. Mapear DTOs y actualizar el stock de productos dentro de un bucle optimizado
            foreach (var detalleDto in ventaDto.DetalleVentas)
            {
                var producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
                if (producto == null)
                {
                    return BadRequest($"Producto con ID {detalleDto.ProductoId} no encontrado.");
                }

                // Reducir el stock del producto
                producto.Stock -= detalleDto.Cantidad;

                // Añadir el detalle de la venta a la lista de la venta
                venta.DetalleVentas.Add(new DetalleVenta
                {
                    ProductoId = detalleDto.ProductoId,
                    Cantidad = detalleDto.Cantidad,
                    PrecioUnitario = detalleDto.PrecioUnitario,
                    Subtotal = detalleDto.Cantidad * detalleDto.PrecioUnitario
                });
            }

            // 5. Actualizar el MontoCierre de la sesión de caja
            sesionCaja.MontoCierre += venta.Total;

            // 6. Añadir la venta al contexto
            _context.Ventas.Add(venta);

            // 7. Guardar todos los cambios en la base de datos de una sola vez
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Error de concurrencia. La sesión de caja o un producto ya fue modificado por otro usuario.");
            }
            catch (Exception ex)
            {
                // Manejo de otros errores de base de datos
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }

            // 8. Mapear y devolver la respuesta
            var ventaDTO = _mapper.Map<VentaDTO>(venta);
            return CreatedAtAction(nameof(GetVenta), new { id = venta.VentaId }, ventaDTO);
        }

        // ✅ Método para generar el siguiente código de venta de forma secuencial
        private string GenerarSiguienteCodigo(string? ultimoCodigo)
        {
            if (string.IsNullOrEmpty(ultimoCodigo))
            {
                return "VTA-001";
            }

            var partes = ultimoCodigo.Split('-');
            if (partes.Length != 2 || !int.TryParse(partes[1], out int numero))
            {
                // En caso de formato inesperado, regresamos al código inicial
                return "VTA-001";
            }

            return $"VTA-{(numero + 1):000}";
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
