using AutoMapper;
using BackendAE.Models;
using BackendAE.DTOs;

namespace BackendAE.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Usuarios
            //CreateMap<Usuario, UsuarioDTO>()
            //    .ForMember(dest => dest.NombreUsuario, opt =>
            //        opt.MapFrom(src => $"{src.PrimerNombre} {src.PrimerApellido}"))
            //    .ForMember(dest => dest.RolNombre, opt =>
            //        opt.MapFrom(src => src.Rol != null ? src.Rol.RolNombre : null));
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(dest => dest.PrimerNombre, opt => opt.MapFrom(src => src.PrimerNombre))
                .ForMember(dest => dest.SegundoNombre, opt => opt.MapFrom(src => src.SegundoNombre))
                .ForMember(dest => dest.PrimerApellido, opt => opt.MapFrom(src => src.PrimerApellido))
                .ForMember(dest => dest.SegundoApellido, opt => opt.MapFrom(src => src.SegundoApellido))
                .ForMember(dest => dest.NIT, opt => opt.MapFrom(src => src.NIT))
                .ForMember(dest => dest.CUI, opt => opt.MapFrom(src => src.CUI))
                .ForMember(dest => dest.FechaNacimiento, opt => opt.MapFrom(src => src.FechaNacimiento))
                .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.Telefono))
                .ForMember(dest => dest.Direccion, opt => opt.MapFrom(src => src.Direccion))
                .ForMember(dest => dest.Genero, opt => opt.MapFrom(src => src.Genero))
                .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.NombreUsuario))
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.RolId, opt => opt.MapFrom(src => src.RolId))
                .ForMember(dest => dest.RolNombre, opt => opt.MapFrom(src => src.Rol != null ? src.Rol.RolNombre : null));


            CreateMap<UsuarioCreacionDTO, Usuario>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<UsuarioActualizacionDTO, Usuario>();
            // Roles
            CreateMap<Rol, RolDTO>();
            CreateMap<RolDTO, Rol>();
            CreateMap<RolDTOCrear, Rol>(); // <<--- este faltaba
            // Productos
            CreateMap<Producto, ProductoDTO>()
            .ForMember(dest => dest.NombreCategoria, opt =>
             opt.MapFrom(src => src.CategoriaProducto != null ? src.CategoriaProducto.Nombre : null));

            CreateMap<ProductoCreacionDTO, Producto>();

            // Categorías de Productos
            CreateMap<CategoriaProducto, CategoriaProductoDTO>();
            CreateMap<CategoriaProductoCreacionDTO, CategoriaProducto>();

            // Proveedores
            CreateMap<Proveedor, ProveedorDTO>()
            .ForMember(dest => dest.NombreCategoria, opt =>
            opt.MapFrom(src => src.CategoriaProveedor != null ? src.CategoriaProveedor.Nombre : null));

            CreateMap<ProveedorCreacionDTO, Proveedor>();

            // Categorías de Proveedores
            CreateMap<CategoriaProveedor, CategoriaProveedorDTO>();
            CreateMap<CategoriaProveedorCreacionDTO, CategoriaProveedor>();

            // Compras
            CreateMap<Compra, CompraDTO>()
            .ForMember(dest => dest.NombreEncargado, opt =>
                opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.NombreEncargado : null))
             .ForMember(dest => dest.DetalleCompras, opt =>
                opt.MapFrom(src => src.DetalleCompras));

            CreateMap<CompraCreacionDTO, Compra>();

            CreateMap<DetalleCompra, DetalleCompraDTO>()
                .ForMember(dest => dest.NombreProducto, opt =>
                    opt.MapFrom(src => src.Producto != null ? src.Producto.Nombre : null));

            CreateMap<DetalleCompraCreacionDTO, DetalleCompra>();

            // Ventas
            // Mapear de entidad a DTO
            // Mapeo bidireccional de Venta a VentaDTO y viceversa
            // Mapeo de Entidad a DTO
            CreateMap<Venta, VentaDTO>()
                .ForMember(dest => dest.DetalleVentas, opt => opt.MapFrom(src => src.DetalleVentas));

            // Mapeo de DTO a Entidad
            CreateMap<VentaCreacionDTO, Venta>()
                .ForMember(dest => dest.VentaId, opt => opt.Ignore())
                .ForMember(dest => dest.CodigoVenta, opt => opt.Ignore())
                .ForMember(dest => dest.FechaVenta, opt => opt.Ignore())
                //.ForMember(dest => dest.EstadoVenta, opt => opt.Ignore())
                .ForMember(dest => dest.DetalleVentas, opt => opt.MapFrom(src => src.DetalleVentas));

            // Mapeo bidireccional para el detalle de la venta

            // Mapeo de DTO a Entidad (el que ya tenías)
            CreateMap<DetalleVentaDTO, DetalleVenta>();

            // ESTE ES EL MAPEO QUE FALTABA Y CAUSABA EL ERROR
            // Mapeo de Entidad a DTO (necesario para la respuesta de la API)
            CreateMap<DetalleVenta, DetalleVentaDTO>();

            // De DTO de creación a entidad
            CreateMap<DetalleVentaCreacionDTO, DetalleVenta>()
                .ForMember(dest => dest.DetalleVentaId, opt => opt.Ignore()) // generado por DB
                .ForMember(dest => dest.VentaId, opt => opt.Ignore())        // lo agregas manualmente en backend si hace falta
                .ForMember(dest => dest.PrecioUnitario, opt => opt.MapFrom(src => src.PrecioUnitario ?? 0))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal ?? 0));

            // Categoria Producto
            CreateMap<CategoriaProducto, CategoriaProductoDTO>();
            CreateMap<CategoriaProductoCreacionDTO, CategoriaProducto>();
            // Caja
            CreateMap<Caja, CajaDTO>();
            CreateMap<CajaCreacionDTO, Caja>();
            // CajaSesion
            CreateMap<CajaSesion, CajaSesionDTO>()
            .ForMember(dest => dest.NombreCaja, opt =>
                opt.MapFrom(src => src.Caja != null ? src.Caja.Nombre : null));
            //.ForMember(dest => dest.UsuarioId, opt =>
            //    opt.MapFrom(src => src.UsuarioCierre != null ? src.UsuarioCierre.UsuarioId : (int?)null));

            // Mapear de DTO de creación a entidad
            CreateMap<CajaSesionCreacionDTO, CajaSesion>();
            // MovimientoCaja
            
            CreateMap<MovimientoCaja, MovimientoCajaDTO>();

            
            CreateMap<MovimientoCajaCreacionDTO, MovimientoCaja>();


            // DetalleCompra
            CreateMap<DetalleCompra, DetalleCompraDTO>()
                .ForMember(dest => dest.NombreProducto, opt =>
                    opt.MapFrom(src => src.Producto != null ? src.Producto.Nombre : null));
            // DetalleVenta
            

            // Aquí irían más mapeos (Producto, Categoria, Proveedor, etc.)
        }
    }
}
