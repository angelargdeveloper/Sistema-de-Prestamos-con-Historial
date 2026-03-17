using AlMaximo.Core.DTOs;
using AlMaximo.Core.Entities;
using AlMaximo.Core.Interfaces;
using AlMaximo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace AlMaximo.Infrastructure.Repositories
{
    public class PrestamoRepository : IPrestamoRepository
    {
        private readonly AlMaximoDbContext _context;

        public PrestamoRepository(AlMaximoDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<PrestamoDto>> GetPrestamosAsync(string? busqueda = null, int? empleadoId = null, int pageNumber = 1, int pageSize = 20)
        {
            try
            {
                // Debug: verificar la conexión y la base de datos
                System.Diagnostics.Debug.WriteLine($"DEBUG: Connection String: {_context.Database.GetConnectionString()}");
                System.Diagnostics.Debug.WriteLine($"DEBUG: Database exists: {await _context.Database.CanConnectAsync()}");
                
                // Debug: verificar diferentes formas de consultar préstamos activos
                var totalPrestamos = await _context.Prestamos.CountAsync();
                System.Diagnostics.Debug.WriteLine($"DEBUG: Total préstamos en tabla: {totalPrestamos}");
                
                // Si no hay préstamos, verifiquemos si hay empleados
                var totalEmpleados = await _context.Empleados.CountAsync();
                System.Diagnostics.Debug.WriteLine($"DEBUG: Total empleados en tabla: {totalEmpleados}");
                
                // Verificar si podemos hacer un query simple directo a la tabla
                var prestamosRaw = await _context.Prestamos.FromSqlRaw("SELECT * FROM Prestamos").CountAsync();
                System.Diagnostics.Debug.WriteLine($"DEBUG: Préstamos con SQL directo: {prestamosRaw}");
                
                var totalPrestamosActivos1 = await _context.Prestamos.Where(p => p.Activo == true).CountAsync();
                System.Diagnostics.Debug.WriteLine($"DEBUG: Activo == true: {totalPrestamosActivos1}");
                
                var totalPrestamosActivos2 = await _context.Prestamos.Where(p => p.Activo).CountAsync();
                System.Diagnostics.Debug.WriteLine($"DEBUG: Solo Activo: {totalPrestamosActivos2}");
                
                var prestamos = await _context.Prestamos
                    .Include(p => p.Empleado)
                    .Include(p => p.TipoPagoAbono)
                    .Include(p => p.AutorPersonaQueAutoriza)
                        .ThenInclude(a => a.Empleado)
                    .Select(p => new PrestamoDto
                    {
                        IdPrestamo = p.IdPrestamo,
                        EmpleadoId = p.EmpleadoId,
                        NombreEmpleado = p.Empleado != null ? $"{p.Empleado.Nombres} {p.Empleado.Apellido1} {p.Empleado.Apellido2}".Trim() : "Sin nombre",
                        NumNomina = p.Empleado != null ? p.Empleado.NumNomina : "Sin nómina",
                        CantidadTotalPrestada = p.CantidadTotalPrestada,
                        CantidadTotalAPagar = p.CantidadTotalAPagar,
                        InteresAprobado = p.InteresAprobado,
                        InteresMoratorio = p.InteresMoratorio,
                        TipoPagoAbonoId = p.TipoPagoAbonoId,
                        TipoPago = p.TipoPagoAbono != null ? p.TipoPagoAbono.NombreCorto : "Sin tipo",
                        DescripcionTipoPago = p.TipoPagoAbono != null ? p.TipoPagoAbono.Descripcion : "Sin descripción",
                        FechaPrimerPago = p.FechaPrimerPago,
                        TotalAbonadoCapital = p.TotalAbonadoCapital,
                        TotalAbonadoIntereses = p.TotalAbonadoIntereses,
                        Saldo = p.Saldo,
                        FechaFinalPago = p.FechaFinalPago,
                        AutorPersonaQueAutorizaId = p.AutorPersonaQueAutorizaId,
                        NombreAutorizador = p.AutorPersonaQueAutoriza != null && p.AutorPersonaQueAutoriza.Empleado != null ? 
                            $"{p.AutorPersonaQueAutoriza.Empleado.Nombres} {p.AutorPersonaQueAutoriza.Empleado.Apellido1} {p.AutorPersonaQueAutoriza.Empleado.Apellido2}".Trim() : "Sin autorizador",
                        Notas = p.Notas,
                        Activo = p.Activo,
                        FechaCreacion = p.FechaCreacion,
                        FechaModificacion = p.FechaModificacion,
                        UsuarioCreacion = p.UsuarioCreacion,
                        UsuarioModificacion = p.UsuarioModificacion
                    })
                    .OrderByDescending(x => x.FechaCreacion)
                    .ToListAsync();

                System.Diagnostics.Debug.WriteLine($"DEBUG: Préstamos DTO creados: {prestamos.Count}");

                // Aplicar filtros después de la consulta inicial
                var filteredItems = prestamos.AsEnumerable()
                    .Where(x => x.Activo); // Filtrar por activo aquí

                if (empleadoId.HasValue)
                {
                    filteredItems = filteredItems.Where(x => x.EmpleadoId == empleadoId.Value);
                }

                if (!string.IsNullOrEmpty(busqueda))
                {
                    filteredItems = filteredItems.Where(x => 
                        x.NombreEmpleado.Contains(busqueda) ||
                        x.NumNomina.Contains(busqueda) ||
                        x.IdPrestamo.ToString().Contains(busqueda));
                }

                var totalItems = filteredItems.Count();
                var pagedItems = filteredItems
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PagedResultDto<PrestamoDto>
                {
                    Items = pagedItems,
                    TotalItems = totalItems,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                // Log del error para diagnóstico
                System.Diagnostics.Debug.WriteLine($"Error in GetPrestamosAsync: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        public async Task<PrestamoDto?> GetPrestamoByIdAsync(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Empleado)
                .Include(p => p.TipoPagoAbono)
                .Include(p => p.AutorPersonaQueAutoriza)
                    .ThenInclude(a => a.Empleado)
                .Where(p => p.IdPrestamo == id)
                .Select(p => new PrestamoDto
                {
                    IdPrestamo = p.IdPrestamo,
                    EmpleadoId = p.EmpleadoId,
                    NombreEmpleado = p.Empleado.Nombres + " " + p.Empleado.Apellido1 + 
                                   (p.Empleado.Apellido2 != null ? " " + p.Empleado.Apellido2 : ""),
                    NumNomina = p.Empleado.NumNomina,
                    CantidadTotalPrestada = p.CantidadTotalPrestada,
                    CantidadTotalAPagar = p.CantidadTotalAPagar,
                    InteresAprobado = p.InteresAprobado,
                    InteresMoratorio = p.InteresMoratorio,
                    TipoPagoAbonoId = p.TipoPagoAbonoId,
                    TipoPago = p.TipoPagoAbono.NombreCorto,
                    DescripcionTipoPago = p.TipoPagoAbono.Descripcion,
                    FechaPrimerPago = p.FechaPrimerPago,
                    TotalAbonadoCapital = p.TotalAbonadoCapital,
                    TotalAbonadoIntereses = p.TotalAbonadoIntereses,
                    Saldo = p.Saldo,
                    FechaFinalPago = p.FechaFinalPago,
                    AutorPersonaQueAutorizaId = p.AutorPersonaQueAutorizaId,
                    NombreAutorizador = p.AutorPersonaQueAutoriza.Empleado.Nombres + " " + 
                                       p.AutorPersonaQueAutoriza.Empleado.Apellido1 + 
                                       (p.AutorPersonaQueAutoriza.Empleado.Apellido2 != null ? " " + p.AutorPersonaQueAutoriza.Empleado.Apellido2 : ""),
                    Notas = p.Notas,
                    Activo = p.Activo,
                    FechaCreacion = p.FechaCreacion,
                    FechaModificacion = p.FechaModificacion,
                    UsuarioCreacion = p.UsuarioCreacion,
                    UsuarioModificacion = p.UsuarioModificacion
                })
                .FirstOrDefaultAsync();

            return prestamo;
        }

        public async Task<int> CreatePrestamoAsync(CreatePrestamoDto createPrestamoDto)
        {
            // Validaciones
            var empleadoExists = await _context.Empleados
                .AnyAsync(e => e.Id == createPrestamoDto.EmpleadoId && e.Activo);
            if (!empleadoExists)
                throw new InvalidOperationException("El empleado especificado no existe o no está activo.");

            var tipoPagoExists = await _context.TipoPagosAbonos
                .AnyAsync(t => t.Id == createPrestamoDto.TipoPagoAbonoId && t.Activo);
            if (!tipoPagoExists)
                throw new InvalidOperationException("El tipo de pago especificado no existe o no está activo.");

            var autorizadorExists = await _context.EmpleadosAutorizadores
                .AnyAsync(a => a.Id == createPrestamoDto.AutorPersonaQueAutorizaId && a.Activo);
            if (!autorizadorExists)
                throw new InvalidOperationException("El empleado autorizador especificado no existe o no está activo.");

            // Crear el préstamo
            var prestamo = new Prestamo
            {
                EmpleadoId = createPrestamoDto.EmpleadoId,
                CantidadTotalPrestada = createPrestamoDto.CantidadTotalPrestada,
                CantidadTotalAPagar = createPrestamoDto.CantidadTotalAPagar,
                InteresAprobado = createPrestamoDto.InteresAprobado,
                InteresMoratorio = createPrestamoDto.InteresMoratorio,
                TipoPagoAbonoId = createPrestamoDto.TipoPagoAbonoId,
                FechaPrimerPago = createPrestamoDto.FechaPrimerPago,
                TotalAbonadoCapital = 0.00m,
                TotalAbonadoIntereses = 0.00m,
                Saldo = createPrestamoDto.CantidadTotalAPagar, // El saldo inicial es igual al total a pagar
                FechaFinalPago = createPrestamoDto.FechaFinalPago,
                AutorPersonaQueAutorizaId = createPrestamoDto.AutorPersonaQueAutorizaId,
                Notas = createPrestamoDto.Notas,
                Activo = true,
                FechaCreacion = DateTime.Now,
                UsuarioCreacion = createPrestamoDto.UsuarioCreacion,
                FechaModificacion = DateTime.Now,
                UsuarioModificacion = createPrestamoDto.UsuarioCreacion
            };

            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();

            // Créer entrada en el historial
            var historial = new PrestamoHistorial
            {
                PrestamoId = prestamo.IdPrestamo,
                EmpleadoId = prestamo.EmpleadoId,
                CantidadTotalPrestada = prestamo.CantidadTotalPrestada,
                CantidadTotalAPagar = prestamo.CantidadTotalAPagar,
                InteresAprobado = prestamo.InteresAprobado,
                InteresMoratorio = prestamo.InteresMoratorio,
                TipoPagoAbonoId = prestamo.TipoPagoAbonoId,
                FechaPrimerPago = prestamo.FechaPrimerPago,
                TotalAbonadoCapital = prestamo.TotalAbonadoCapital,
                TotalAbonadoIntereses = prestamo.TotalAbonadoIntereses,
                Saldo = prestamo.Saldo,
                FechaFinalPago = prestamo.FechaFinalPago,
                AutorPersonaQueAutorizaId = prestamo.AutorPersonaQueAutorizaId,
                Notas = prestamo.Notas,
                Activo = true,
                TipoOperacion = "CREATE",
                FechaOperacion = DateTime.Now,
                UsuarioOperacion = createPrestamoDto.UsuarioCreacion
            };

            _context.PrestamosHistorial.Add(historial);
            await _context.SaveChangesAsync();

            return prestamo.IdPrestamo;
        }

        public async Task<List<PrestamoHistorialDto>> GetHistorialPrestamoAsync(int prestamoId)
        {
            var historial = await _context.PrestamosHistorial
                .Include(h => h.Empleado)
                .Include(h => h.TipoPagoAbono)
                .Include(h => h.AutorPersonaQueAutoriza)
                    .ThenInclude(a => a.Empleado)
                .Where(h => h.PrestamoId == prestamoId)
                .OrderByDescending(h => h.FechaOperacion)
                .Select(h => new PrestamoHistorialDto
                {
                    Id = h.Id,
                    PrestamoId = h.PrestamoId,
                    NombreEmpleado = h.Empleado.Nombres + " " + h.Empleado.Apellido1 + 
                                   (h.Empleado.Apellido2 != null ? " " + h.Empleado.Apellido2 : ""),
                    CantidadTotalPrestada = h.CantidadTotalPrestada,
                    CantidadTotalAPagar = h.CantidadTotalAPagar,
                    InteresAprobado = h.InteresAprobado,
                    InteresMoratorio = h.InteresMoratorio,
                    TipoPago = h.TipoPagoAbono.NombreCorto,
                    FechaPrimerPago = h.FechaPrimerPago,
                    TotalAbonadoCapital = h.TotalAbonadoCapital,
                    TotalAbonadoIntereses = h.TotalAbonadoIntereses,
                    Saldo = h.Saldo,
                    FechaFinalPago = h.FechaFinalPago,
                    NombreAutorizador = h.AutorPersonaQueAutoriza.Empleado.Nombres + " " + 
                                       h.AutorPersonaQueAutoriza.Empleado.Apellido1 + 
                                       (h.AutorPersonaQueAutoriza.Empleado.Apellido2 != null ? " " + h.AutorPersonaQueAutoriza.Empleado.Apellido2 : ""),
                    Notas = h.Notas,
                    Activo = h.Activo,
                    TipoOperacion = h.TipoOperacion,
                    FechaOperacion = h.FechaOperacion,
                    UsuarioOperacion = h.UsuarioOperacion
                })
                .ToListAsync();

            return historial;
        }

        public async Task<bool> UpdatePrestamoAsync(UpdatePrestamoDto updatePrestamoDto)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdPrestamo", updatePrestamoDto.IdPrestamo),
                    new SqlParameter("@EmpleadoId", updatePrestamoDto.EmpleadoId),
                    new SqlParameter("@CantidadTotalPrestada", updatePrestamoDto.CantidadTotalPrestada),
                    new SqlParameter("@CantidadTotalAPagar", updatePrestamoDto.CantidadTotalAPagar),
                    new SqlParameter("@InteresAprobado", updatePrestamoDto.InteresAprobado),
                    new SqlParameter("@InteresMoratorio", updatePrestamoDto.InteresMoratorio),
                    new SqlParameter("@TipoPagoAbonoId", updatePrestamoDto.TipoPagoAbonoId),
                    new SqlParameter("@FechaPrimerPago", (object?)updatePrestamoDto.FechaPrimerPago ?? DBNull.Value),
                    new SqlParameter("@TotalAbonadoCapital", updatePrestamoDto.TotalAbonadoCapital),
                    new SqlParameter("@TotalAbonadoIntereses", updatePrestamoDto.TotalAbonadoIntereses),
                    new SqlParameter("@FechaFinalPago", (object?)updatePrestamoDto.FechaFinalPago ?? DBNull.Value),
                    new SqlParameter("@AutorPersonaQueAutorizaId", updatePrestamoDto.AutorPersonaQueAutorizaId),
                    new SqlParameter("@Notas", (object?)updatePrestamoDto.Notas ?? DBNull.Value),
                    new SqlParameter("@UsuarioModificacion", updatePrestamoDto.UsuarioModificacion)
                };

                await _context.Database.ExecuteSqlRawAsync(@"
                    EXEC SP_UpdatePrestamo 
                        @IdPrestamo, 
                        @EmpleadoId, 
                        @CantidadTotalPrestada, 
                        @CantidadTotalAPagar, 
                        @InteresAprobado, 
                        @InteresMoratorio, 
                        @TipoPagoAbonoId, 
                        @FechaPrimerPago, 
                        @TotalAbonadoCapital, 
                        @TotalAbonadoIntereses, 
                        @FechaFinalPago, 
                        @AutorPersonaQueAutorizaId, 
                        @Notas, 
                        @UsuarioModificacion", parameters);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeletePrestamoAsync(int id, string usuarioEliminacion)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@IdPrestamo", id),
                    new SqlParameter("@UsuarioEliminacion", usuarioEliminacion)
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC SP_DeletePrestamo @IdPrestamo, @UsuarioEliminacion", 
                    parameters);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}