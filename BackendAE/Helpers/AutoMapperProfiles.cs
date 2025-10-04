using AutoMapper;
using BackendAE.Models;
using BackendAE.DTOs;
using System;

namespace BackendAE.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // -------------------------------------------------
            //  Usuarios
            // -------------------------------------------------
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(d => d.PrimerNombre, opt => opt.MapFrom(s => s.PrimerNombre))
                .ForMember(d => d.SegundoNombre, opt => opt.MapFrom(s => s.SegundoNombre))
                .ForMember(d => d.PrimerApellido, opt => opt.MapFrom(s => s.PrimerApellido))
                .ForMember(d => d.SegundoApellido, opt => opt.MapFrom(s => s.SegundoApellido))
                .ForMember(d => d.NIT, opt => opt.MapFrom(s => s.NIT))
                .ForMember(d => d.CUI, opt => opt.MapFrom(s => s.CUI))
                .ForMember(d => d.FechaNacimiento, opt => opt.MapFrom(s => s.FechaNacimiento))
                .ForMember(d => d.Telefono, opt => opt.MapFrom(s => s.Telefono))
                .ForMember(d => d.Direccion, opt => opt.MapFrom(s => s.Direccion))
                .ForMember(d => d.Genero, opt => opt.MapFrom(s => s.Genero))
                .ForMember(d => d.NombreUsuario, opt => opt.MapFrom(s => s.NombreUsuario))
                .ForMember(d => d.Estado, opt => opt.MapFrom(s => s.Estado))
                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                .ForMember(d => d.RolId, opt => opt.MapFrom(s => s.RolId))
                .ForMember(d => d.RolNombre, opt => opt.MapFrom(s => s.Rol != null ? s.Rol.RolNombre : null));

            CreateMap<UsuarioCreacionDTO, Usuario>()
                .ForMember(d => d.PasswordHash, opt => opt.Ignore());

            CreateMap<UsuarioActualizacionDTO, Usuario>();

            // -------------------------------------------------
            //  Roles
            // -------------------------------------------------
            CreateMap<Rol, RolDTO>();
            CreateMap<RolDTO, Rol>();
            CreateMap<RolDTOCrear, Rol>();

            // -------------------------------------------------
            //  Productos
            // -------------------------------------------------
            CreateMap<Producto, ProductoDTO>()
                .ForMember(d => d.NombreCategoria,
                           opt => opt.MapFrom(s => s.CategoriaProducto != null ? s.CategoriaProducto.Nombre : null));

            CreateMap<ProductoCreacionDTO, Producto>();

            // -------------------------------------------------
            //  Categorías de Producto
            // -------------------------------------------------
            CreateMap<CategoriaProducto, CategoriaProductoDTO>();
            CreateMap<CategoriaProductoCreacionDTO, CategoriaProducto>();

            // -------------------------------------------------
            //  Proveedores
            // -------------------------------------------------
            CreateMap<Proveedor, ProveedorDTO>()
                .ForMember(d => d.NombreCategoria,
                           opt => opt.MapFrom(s => s.CategoriaProveedor != null ? s.CategoriaProveedor.Nombre : null));

            CreateMap<ProveedorCreacionDTO, Proveedor>();

            // -------------------------------------------------
            //  Categorías de Proveedor
            // -------------------------------------------------
            CreateMap<CategoriaProveedor, CategoriaProveedorDTO>();
            CreateMap<CategoriaProveedorCreacionDTO, CategoriaProveedor>();

            // -------------------------------------------------
            //  COMPRAS  (sin tabla DetalleCompra)
            // -------------------------------------------------
            CreateMap<Compra, CompraDTO>()
                .ForMember(d => d.NombreProveedor,
                    opt => opt.MapFrom(s => s.Proveedor != null ? s.Proveedor.NombreEncargado : null))
                .ForMember(d => d.Total, opt => opt.MapFrom(s => s.Total));

            // <--- NUEVO: mapa de DTO → entidad (para Update) -----------------
            CreateMap<CompraDTO, Compra>()
                // Los campos que tengan el mismo nombre y tipo no necesitan
                // configuración explícita, pero lo dejamos por claridad:
                .ForMember(d => d.CompraId, opt => opt.MapFrom(s => s.CompraId))
                .ForMember(d => d.FechaCompra, opt => opt.MapFrom(s => s.FechaCompra))
                .ForMember(d => d.Observacion, opt => opt.MapFrom(s => s.Observacion))
                .ForMember(d => d.Nombre, opt => opt.MapFrom(s => s.Nombre))
                .ForMember(d => d.Descripcion, opt => opt.MapFrom(s => s.Descripcion))
                .ForMember(d => d.Estado, opt => opt.MapFrom(s => s.Estado))
                .ForMember(d => d.Stock, opt => opt.MapFrom(s => s.Stock))
                .ForMember(d => d.PrecioAdquisicion, opt => opt.MapFrom(s => s.PrecioAdquisicion))
                .ForMember(d => d.PrecioVenta, opt => opt.MapFrom(s => s.PrecioVenta))
                .ForMember(d => d.ProveedorId, opt => opt.MapFrom(s => s.ProveedorId))
                // El total normalmente se calcula en el service, por eso lo ignoramos:
                .ForMember(d => d.Total, opt => opt.Ignore());
            
            CreateMap<CompraCreacionDTO, Compra>();
            // -------------------------------------------------
            //  VENTAS  (sin tabla DetalleVenta)
            // -------------------------------------------------
            // Entidad → DTO
            CreateMap<Venta, VentaDTO>()
                .ForMember(d => d.SubTotal, opt => opt.MapFrom(s => s.SubTotal))
                .ForMember(d => d.CantidadVendida, opt => opt.MapFrom(s => s.CantidadVendida))
                .ForMember(d => d.PrecioUnitario, opt => opt.MapFrom(s => s.PrecioUnitario))
                .ForMember(d => d.ProductoId, opt => opt.MapFrom(s => s.ProductoId));

            // DTO → Entidad (creación)
            CreateMap<VentaCreacionDTO, Venta>()
                .ForMember(d => d.VentaId, opt => opt.Ignore())
                .ForMember(d => d.CodigoVenta, opt => opt.Ignore())
                .ForMember(d => d.FechaVenta, opt => opt.Ignore())
                .ForMember(d => d.EstadoVenta, opt => opt.Ignore())
                .ForMember(d => d.Total, opt => opt.Ignore())
                .ForMember(d => d.SubTotal, opt => opt.Ignore())   // lo calculamos en el Service
                .ForMember(d => d.CajaSesionId, opt => opt.Ignore())
                .ForMember(d => d.UsuarioId, opt => opt.Ignore())
                .ForMember(d => d.ProductoId, opt => opt.MapFrom(s => s.ProductoId))
                .ForMember(d => d.CantidadVendida,
                           opt => opt.MapFrom(s => s.CantidadVendida))
                .ForMember(d => d.PrecioUnitario,
                           opt => opt.MapFrom(s => s.PrecioUnitario));

            // -------------------------------------------------
            //  Caja / CajaSesion / MovimientoCaja
            // -------------------------------------------------
            CreateMap<Caja, CajaDTO>();
            CreateMap<CajaCreacionDTO, Caja>();

            CreateMap<CajaSesion, CajaSesionDTO>()
                .ForMember(d => d.NombreCaja,
                           opt => opt.MapFrom(s => s.Caja != null ? s.Caja.Nombre : null));

            CreateMap<CajaSesionCreacionDTO, CajaSesion>();

            CreateMap<MovimientoCaja, MovimientoCajaDTO>();
            CreateMap<MovimientoCajaCreacionDTO, MovimientoCaja>();

            // -------------------------------------------------
            //  (Recordemos que los mapeos de DetalleCompra / DetalleVenta se eliminaron)
            // -------------------------------------------------
        }
    }
}
