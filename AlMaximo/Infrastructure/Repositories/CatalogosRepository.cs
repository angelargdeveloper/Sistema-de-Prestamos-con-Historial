using AlMaximo.Core.DTOs;
using AlMaximo.Core.Interfaces;
using AlMaximo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace AlMaximo.Infrastructure.Repositories
{
    public class CatalogosRepository : IEmpleadoRepository, IEmpleadoAutorizadorRepository, ITipoPagoAbonoRepository
    {
        private readonly AlMaximoDbContext _context;

        public CatalogosRepository(AlMaximoDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<EmpleadoDto>> GetEmpleadosAsync(string? busqueda = null, int pageNumber = 1, int pageSize = 20)
        {
            var query = _context.Empleados.Where(e => e.Activo);

            // Aplicar búsqueda si se proporciona
            if (!string.IsNullOrEmpty(busqueda))
            {
                query = query.Where(e => 
                    e.Nombres.Contains(busqueda) ||
                    e.Apellido1.Contains(busqueda) ||
                    e.Apellido2.Contains(busqueda) ||
                    e.NumNomina.Contains(busqueda));
            }

            var totalItems = await query.CountAsync();

            var empleados = await query
                .OrderBy(e => e.Apellido1)
                .ThenBy(e => e.Apellido2)
                .ThenBy(e => e.Nombres)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EmpleadoDto
                {
                    Id = e.Id,
                    NumNomina = e.NumNomina,
                    Nombres = e.Nombres,
                    Apellido1 = e.Apellido1,
                    Apellido2 = e.Apellido2,
                    NombreCompleto = e.Nombres + " " + e.Apellido1 + (e.Apellido2 != null ? " " + e.Apellido2 : ""),
                    Activo = e.Activo
                })
                .ToListAsync();

            return new PagedResultDto<EmpleadoDto>
            {
                Items = empleados,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<EmpleadoDto?> GetEmpleadoByIdAsync(int id)
        {
            var empleado = await _context.Empleados
                .Where(e => e.Id == id && e.Activo)
                .Select(e => new EmpleadoDto
                {
                    Id = e.Id,
                    NumNomina = e.NumNomina,
                    Nombres = e.Nombres,
                    Apellido1 = e.Apellido1,
                    Apellido2 = e.Apellido2,
                    NombreCompleto = e.Nombres + " " + e.Apellido1 + (e.Apellido2 != null ? " " + e.Apellido2 : ""),
                    Activo = e.Activo
                })
                .FirstOrDefaultAsync();

            return empleado;
        }

        public async Task<List<EmpleadoAutorizadorDto>> GetEmpleadosAutorizadoresAsync()
        {
            var autorizadores = await _context.Database
                .SqlQueryRaw<EmpleadoAutorizadorDto>("EXEC SP_GetEmpleadosAutorizadores")
                .ToListAsync();

            return autorizadores;
        }

        public async Task<List<TipoPagoAbonoDto>> GetTiposPagoAbonoAsync()
        {
            var tiposPago = await _context.Database
                .SqlQueryRaw<TipoPagoAbonoDto>("EXEC SP_GetTiposPagoAbono")
                .ToListAsync();

            return tiposPago;
        }
    }
}