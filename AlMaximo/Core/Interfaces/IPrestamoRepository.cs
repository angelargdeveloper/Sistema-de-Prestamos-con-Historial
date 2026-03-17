using AlMaximo.Core.DTOs;

namespace AlMaximo.Core.Interfaces
{
    public interface IPrestamoRepository
    {
        Task<PagedResultDto<PrestamoDto>> GetPrestamosAsync(string? busqueda = null, int? empleadoId = null, int pageNumber = 1, int pageSize = 20);
        Task<PrestamoDto?> GetPrestamoByIdAsync(int id);
        Task<int> CreatePrestamoAsync(CreatePrestamoDto createPrestamoDto);
        Task<bool> UpdatePrestamoAsync(UpdatePrestamoDto updatePrestamoDto);
        Task<bool> DeletePrestamoAsync(int id, string usuarioEliminacion);
        Task<List<PrestamoHistorialDto>> GetHistorialPrestamoAsync(int prestamoId);
    }
}