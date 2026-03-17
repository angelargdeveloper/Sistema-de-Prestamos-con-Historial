using AlMaximo.Core.DTOs;

namespace AlMaximo.Core.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<PagedResultDto<EmpleadoDto>> GetEmpleadosAsync(string? busqueda = null, int pageNumber = 1, int pageSize = 20);
        Task<EmpleadoDto?> GetEmpleadoByIdAsync(int id);
    }

    public interface IEmpleadoAutorizadorRepository
    {
        Task<List<EmpleadoAutorizadorDto>> GetEmpleadosAutorizadoresAsync();
    }

    public interface ITipoPagoAbonoRepository
    {
        Task<List<TipoPagoAbonoDto>> GetTiposPagoAbonoAsync();
    }
}