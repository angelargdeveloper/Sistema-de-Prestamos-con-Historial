using AutoMapper;
using AlMaximo.Core.Entities;
using AlMaximo.Core.DTOs;

namespace AlMaximo.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Empleado mappings
            CreateMap<Empleado, EmpleadoDto>()
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => 
                    src.Nombres + " " + src.Apellido1 + (string.IsNullOrEmpty(src.Apellido2) ? "" : " " + src.Apellido2)));

            // EmpleadoAutorizador mappings
            CreateMap<EmpleadoAutorizador, EmpleadoAutorizadorDto>()
                .ForMember(dest => dest.NumNomina, opt => opt.MapFrom(src => src.Empleado.NumNomina))
                .ForMember(dest => dest.NombreCompleto, opt => opt.MapFrom(src => 
                    src.Empleado.Nombres + " " + src.Empleado.Apellido1 + 
                    (string.IsNullOrEmpty(src.Empleado.Apellido2) ? "" : " " + src.Empleado.Apellido2)));

            // TipoPagoAbono mappings
            CreateMap<TipoPagoAbono, TipoPagoAbonoDto>();

            // Prestamo mappings
            CreateMap<Prestamo, PrestamoDto>()
                .ForMember(dest => dest.NombreEmpleado, opt => opt.MapFrom(src => 
                    src.Empleado.Nombres + " " + src.Empleado.Apellido1 + 
                    (string.IsNullOrEmpty(src.Empleado.Apellido2) ? "" : " " + src.Empleado.Apellido2)))
                .ForMember(dest => dest.NumNomina, opt => opt.MapFrom(src => src.Empleado.NumNomina))
                .ForMember(dest => dest.TipoPago, opt => opt.MapFrom(src => src.TipoPagoAbono.NombreCorto))
                .ForMember(dest => dest.DescripcionTipoPago, opt => opt.MapFrom(src => src.TipoPagoAbono.Descripcion))
                .ForMember(dest => dest.NombreAutorizador, opt => opt.MapFrom(src => 
                    src.AutorPersonaQueAutoriza.Empleado.Nombres + " " + 
                    src.AutorPersonaQueAutoriza.Empleado.Apellido1 + 
                    (string.IsNullOrEmpty(src.AutorPersonaQueAutoriza.Empleado.Apellido2) ? "" : 
                     " " + src.AutorPersonaQueAutoriza.Empleado.Apellido2)));

            CreateMap<CreatePrestamoDto, Prestamo>()
                .ForMember(dest => dest.IdPrestamo, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAbonadoCapital, opt => opt.MapFrom(src => 0.00m))
                .ForMember(dest => dest.TotalAbonadoIntereses, opt => opt.MapFrom(src => 0.00m))
                .ForMember(dest => dest.Saldo, opt => opt.MapFrom(src => src.CantidadTotalAPagar))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UsuarioModificacion, opt => opt.MapFrom(src => src.UsuarioCreacion));

            CreateMap<UpdatePrestamoDto, Prestamo>()
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Saldo, opt => opt.MapFrom(src => 
                    src.CantidadTotalAPagar - (src.TotalAbonadoCapital + src.TotalAbonadoIntereses)));

            // PrestamoHistorial mappings
            CreateMap<PrestamoHistorial, PrestamoDto>()
                .ForMember(dest => dest.IdPrestamo, opt => opt.MapFrom(src => src.PrestamoId))
                .ForMember(dest => dest.NombreEmpleado, opt => opt.MapFrom(src => 
                    src.Empleado.Nombres + " " + src.Empleado.Apellido1 + 
                    (string.IsNullOrEmpty(src.Empleado.Apellido2) ? "" : " " + src.Empleado.Apellido2)))
                .ForMember(dest => dest.NumNomina, opt => opt.MapFrom(src => src.Empleado.NumNomina))
                .ForMember(dest => dest.TipoPago, opt => opt.MapFrom(src => src.TipoPagoAbono.NombreCorto))
                .ForMember(dest => dest.DescripcionTipoPago, opt => opt.MapFrom(src => src.TipoPagoAbono.Descripcion))
                .ForMember(dest => dest.NombreAutorizador, opt => opt.MapFrom(src => 
                    src.AutorPersonaQueAutoriza.Empleado.Nombres + " " + 
                    src.AutorPersonaQueAutoriza.Empleado.Apellido1 + 
                    (string.IsNullOrEmpty(src.AutorPersonaQueAutoriza.Empleado.Apellido2) ? "" : 
                     " " + src.AutorPersonaQueAutoriza.Empleado.Apellido2)))
                .ForMember(dest => dest.FechaCreacion, opt => opt.MapFrom(src => src.FechaOperacion))
                .ForMember(dest => dest.FechaModificacion, opt => opt.MapFrom(src => src.FechaOperacion))
                .ForMember(dest => dest.UsuarioCreacion, opt => opt.MapFrom(src => src.UsuarioOperacion))
                .ForMember(dest => dest.UsuarioModificacion, opt => opt.MapFrom(src => src.UsuarioOperacion));
        }
    }
}