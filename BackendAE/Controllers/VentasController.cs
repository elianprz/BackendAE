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

        // --- GETs y PUT/DELETE se mantienen sin cambios ---

        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDTO>>> GetVentas()
        {
            var ventas = await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.CajaSesion)
                .ToListAsync();

            return Ok(_mapper.Map<List<VentaDTO>>(ventas));
        }

        [Authorize(Roles = "Admin,Empleado")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<VentaDTO>> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.CajaSesion)
                .FirstOrDefaultAsync(v => v.VentaId == id);

            if (venta == null) return NotFound();

            return Ok(_mapper.Map<VentaDTO>(venta));
        }

        // -------------------------------------------------
        //  POST: api/Ventas (Creación por Lote)
        // -------------------------------------------------
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPost]
        public async Task<ActionResult<List<VentaDTO>>> PostVenta([FromBody] VentaLoteCreacionDTO ventaLoteDto)
        {
            if (ventaLoteDto.Items == null || !ventaLoteDto.Items.Any())
                return BadRequest("La lista de productos en el carrito no puede estar vacía.");

            // 1️⃣ Obtener y validar el Id del usuario autenticado
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioId) || !await _context.Usuarios.AnyAsync(u => u.UsuarioId == usuarioId))
                return BadRequest("Usuario no válido o no encontrado.");

            // 2️⃣ Generar un CÓDIGO DE VENTA ÚNICO basado en tiempo (Solución robusta a largo plazo)
            // Esto asegura unicidad incluso sin consultar la DB.
            var codigoVenta = $"VNT-{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

            // 3️⃣ Obtener los datos generales
            decimal totalVentaGeneral = ventaLoteDto.Total;
            int? cajaSesionId = ventaLoteDto.CajaSesionId;

            // 4️⃣ Validar y cargar la sesión de caja
            CajaSesion? sesionCaja = null;
            if (cajaSesionId.HasValue && cajaSesionId.Value > 0)
            {
                sesionCaja = await _context.CajaSesiones.FindAsync(cajaSesionId.Value);
                if (sesionCaja == null)
                    return BadRequest("Sesión de caja no encontrada o no válida.");

                // Asegurar que la entidad esté rastreada para poder modificarla
                _context.CajaSesiones.Attach(sesionCaja);
            }

            var ventasToAdd = new List<Venta>();

            // 5️⃣ INICIO DE LA TRANSACCIÓN
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var itemDto in ventaLoteDto.Items)
                {
                    var producto = await _context.Productos.FindAsync(itemDto.ProductoId);
                    if (producto == null)
                        return BadRequest($"Producto con Id {itemDto.ProductoId} no encontrado.");

                    if (producto.Stock < itemDto.CantidadVendida)
                        return BadRequest($"Stock insuficiente para producto {producto.Nombre}. Disponible: {producto.Stock}");

                    var subTotal = itemDto.CantidadVendida * itemDto.PrecioUnitario;

                    // Crear la entidad Venta
                    var venta = new Venta
                    {
                        FechaVenta = DateTime.UtcNow,
                        CodigoVenta = codigoVenta, // Usa el código ÚNICO generado arriba

                        // Campos de Encabezado (Del Lote DTO)
                        Total = totalVentaGeneral,
                        EfectivoRecibido = ventaLoteDto.EfectivoRecibido,
                        Cambio = ventaLoteDto.Cambio,
                        EstadoVenta = ventaLoteDto.EstadoVenta,
                        UsuarioId = usuarioId,
                        CajaSesionId = cajaSesionId,

                        // Campos de Detalle (Del Ítem DTO)
                        ProductoId = itemDto.ProductoId,
                        CantidadVendida = itemDto.CantidadVendida,
                        PrecioUnitario = itemDto.PrecioUnitario,
                        SubTotal = subTotal
                    };

                    // Restar stock y asegurar el rastreo para guardar el cambio
                    producto.Stock -= itemDto.CantidadVendida;
                    _context.Entry(producto).State = EntityState.Modified;

                    ventasToAdd.Add(venta);
                }

                // 6️⃣ Actualizar el MontoCierre (Solo si la sesión fue encontrada)
                if (sesionCaja != null)
                {
                    sesionCaja.MontoCierre += totalVentaGeneral;
                    _context.Entry(sesionCaja).State = EntityState.Modified;
                }

                // 7️⃣ Persistir
                _context.Ventas.AddRange(ventasToAdd);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetVentas), _mapper.Map<List<VentaDTO>>(ventasToAdd));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // 🚨 Búsqueda de la Inner Exception para debug
                var innerEx = ex;
                while (innerEx.InnerException != null)
                {
                    innerEx = innerEx.InnerException;
                }

                // ⚠️ Devolvemos el error real.
                // Una vez resuelto, regresa a "Error interno al procesar la venta por lote."
                return StatusCode(500, $"DB Save Error: {innerEx.Message}");
            }
        }

        // -------------------------------------------------
        //  PUT: api/Ventas/estado/{codigoVenta}
        // -------------------------------------------------
        [Authorize(Roles = "Admin,Empleado")]
        [HttpPut("estado/{codigoVenta}")]
        public async Task<ActionResult> ActualizarEstadoVenta(string codigoVenta, [FromBody] VentaEstadoUpdateDTO dto)
        {
            if (string.IsNullOrEmpty(dto.EstadoVenta))
                return BadRequest("El estado de la venta no puede estar vacío.");

            var ventasBd = await _context.Ventas
                .Where(v => v.CodigoVenta == codigoVenta)
                .ToListAsync();

            if (!ventasBd.Any())
                return NotFound($"Venta con código {codigoVenta} no encontrada.");

            foreach (var venta in ventasBd)
            {
                venta.EstadoVenta = dto.EstadoVenta;
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // -------------------------------------------------
        //  DELETE: api/Ventas/transaccion/{codigoVenta}
        // -------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpDelete("transaccion/{codigoVenta}")]
        public async Task<ActionResult> EliminarTransaccion(string codigoVenta)
        {
            var ventas = await _context.Ventas
                .Where(v => v.CodigoVenta == codigoVenta)
                .ToListAsync();

            if (!ventas.Any())
                return NotFound($"Venta con código {codigoVenta} no encontrada.");

            _context.Ventas.RemoveRange(ventas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // -------------------------------------------------
        //  HELPERS
        // -------------------------------------------------
        // ✅ Este método ya no es necesario, pero lo dejo aquí si lo usas en otro lado
        private string GenerarSiguienteCodigo(string? ultimoCodigo)
        {
            if (string.IsNullOrEmpty(ultimoCodigo) || !ultimoCodigo.StartsWith("VTA-"))
                return "VTA-001";

            var partes = ultimoCodigo.Split('-');
            if (partes.Length != 2 || !int.TryParse(partes[1], out int numero))
                return "VTA-001";

            return $"VTA-{(numero + 1):000}";
        }
    }
}