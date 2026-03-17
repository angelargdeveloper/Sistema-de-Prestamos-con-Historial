using AlMaximo.Core.DTOs;

namespace AlMaximo.Core.Interfaces
{
    public interface IPrestamoService
    {
        Task<PagedResultDto<PrestamoDto>> GetPrestamosAsync(SearchParametersDto searchParameters, int? empleadoId = null);
        Task<PrestamoDto?> GetPrestamoByIdAsync(int id);
        Task<ApiResponseDto<int>> CreatePrestamoAsync(CreatePrestamoDto createPrestamoDto);
        Task<ApiResponseDto<bool>> UpdatePrestamoAsync(UpdatePrestamoDto updatePrestamoDto);
        Task<ApiResponseDto<bool>> DeletePrestamoAsync(int id, string usuarioEliminacion);
        Task<List<PrestamoHistorialDto>> GetHistorialPrestamoAsync(int prestamoId);
    }

    public interface ICatalogosService
    {
        Task<PagedResultDto<EmpleadoDto>> GetEmpleadosAsync(SearchParametersDto searchParameters);
        Task<List<EmpleadoAutorizadorDto>> GetEmpleadosAutorizadoresAsync();
        Task<List<TipoPagoAbonoDto>> GetTiposPagoAbonoAsync();
    }
}