using AlMaximo.Core.DTOs;
using AlMaximo.Core.Interfaces;

namespace AlMaximo.Application.Services
{
    public class CatalogosService : ICatalogosService
    {
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly IEmpleadoAutorizadorRepository _empleadoAutorizadorRepository;
        private readonly ITipoPagoAbonoRepository _tipoPagoAbonoRepository;

        public CatalogosService(
            IEmpleadoRepository empleadoRepository,
            IEmpleadoAutorizadorRepository empleadoAutorizadorRepository,
            ITipoPagoAbonoRepository tipoPagoAbonoRepository)
        {
            _empleadoRepository = empleadoRepository;
            _empleadoAutorizadorRepository = empleadoAutorizadorRepository;
            _tipoPagoAbonoRepository = tipoPagoAbonoRepository;
        }

        public async Task<PagedResultDto<EmpleadoDto>> GetEmpleadosAsync(SearchParametersDto searchParameters)
        {
            return await _empleadoRepository.GetEmpleadosAsync(
                searchParameters.Busqueda,
                searchParameters.PageNumber,
                searchParameters.PageSize);
        }

        public async Task<List<EmpleadoAutorizadorDto>> GetEmpleadosAutorizadoresAsync()
        {
            return await _empleadoAutorizadorRepository.GetEmpleadosAutorizadoresAsync();
        }

        public async Task<List<TipoPagoAbonoDto>> GetTiposPagoAbonoAsync()
        {
            return await _tipoPagoAbonoRepository.GetTiposPagoAbonoAsync();
        }
    }
}