using AlMaximo.Core.DTOs;
using AlMaximo.Core.Interfaces;
using FluentValidation;

namespace AlMaximo.Application.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly IPrestamoRepository _prestamoRepository;
        private readonly IValidator<CreatePrestamoDto> _createPrestamoValidator;
        private readonly IValidator<UpdatePrestamoDto> _updatePrestamoValidator;

        public PrestamoService(
            IPrestamoRepository prestamoRepository,
            IValidator<CreatePrestamoDto> createPrestamoValidator,
            IValidator<UpdatePrestamoDto> updatePrestamoValidator)
        {
            _prestamoRepository = prestamoRepository;
            _createPrestamoValidator = createPrestamoValidator;
            _updatePrestamoValidator = updatePrestamoValidator;
        }

        public async Task<PagedResultDto<PrestamoDto>> GetPrestamosAsync(SearchParametersDto searchParameters, int? empleadoId = null)
        {
            return await _prestamoRepository.GetPrestamosAsync(
                searchParameters.Busqueda,
                empleadoId,
                searchParameters.PageNumber,
                searchParameters.PageSize);
        }

        public async Task<PrestamoDto?> GetPrestamoByIdAsync(int id)
        {
            return await _prestamoRepository.GetPrestamoByIdAsync(id);
        }

        public async Task<ApiResponseDto<int>> CreatePrestamoAsync(CreatePrestamoDto createPrestamoDto)
        {
            var response = new ApiResponseDto<int>();

            try
            {
                // Validar el DTO
                var validationResult = await _createPrestamoValidator.ValidateAsync(createPrestamoDto);
                if (!validationResult.IsValid)
                {
                    response.Success = false;
                    response.Message = "Errores de validación";
                    response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return response;
                }

                // Validaciones de negocio adicionales
                var businessValidationErrors = await ValidateBusinessRulesForCreate(createPrestamoDto);
                if (businessValidationErrors.Any())
                {
                    response.Success = false;
                    response.Message = "Errores de reglas de negocio";
                    response.Errors = businessValidationErrors;
                    return response;
                }

                // Crear el préstamo
                var prestamoId = await _prestamoRepository.CreatePrestamoAsync(createPrestamoDto);

                response.Success = true;
                response.Message = "Préstamo creado exitosamente";
                response.Data = prestamoId;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error interno del servidor";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<ApiResponseDto<bool>> UpdatePrestamoAsync(UpdatePrestamoDto updatePrestamoDto)
        {
            var response = new ApiResponseDto<bool>();

            try
            {
                // Validar el DTO
                var validationResult = await _updatePrestamoValidator.ValidateAsync(updatePrestamoDto);
                if (!validationResult.IsValid)
                {
                    response.Success = false;
                    response.Message = "Errores de validación";
                    response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return response;
                }

                // Verificar que el préstamo existe
                var existingPrestamo = await _prestamoRepository.GetPrestamoByIdAsync(updatePrestamoDto.IdPrestamo);
                if (existingPrestamo == null)
                {
                    response.Success = false;
                    response.Message = "El préstamo especificado no existe";
                    return response;
                }

                // Validaciones de negocio adicionales
                var businessValidationErrors = await ValidateBusinessRulesForUpdate(updatePrestamoDto);
                if (businessValidationErrors.Any())
                {
                    response.Success = false;
                    response.Message = "Errores de reglas de negocio";
                    response.Errors = businessValidationErrors;
                    return response;
                }

                // Actualizar el préstamo
                var result = await _prestamoRepository.UpdatePrestamoAsync(updatePrestamoDto);

                response.Success = result;
                response.Message = result ? "Préstamo actualizado exitosamente" : "Error al actualizar el préstamo";
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error interno del servidor";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }

        public async Task<ApiResponseDto<bool>> DeletePrestamoAsync(int id, string usuarioEliminacion)
        {
            var response = new ApiResponseDto<bool>();

            try
            {
                // Verificar que el préstamo existe
                var existingPrestamo = await _prestamoRepository.GetPrestamoByIdAsync(id);
                if (existingPrestamo == null)
                {
                    response.Success = false;
                    response.Message = "El préstamo especificado no existe";
                    return response;
                }

                // Validar si el préstamo se puede eliminar
                if (existingPrestamo.TotalAbonadoCapital > 0 || existingPrestamo.TotalAbonadoIntereses > 0)
                {
                    response.Success = false;
                    response.Message = "No se puede eliminar un préstamo que ya tiene abonos realizados";
                    return response;
                }

                // Eliminar el préstamo
                var result = await _prestamoRepository.DeletePrestamoAsync(id, usuarioEliminacion);

                response.Success = result;
                response.Message = result ? "Préstamo eliminado exitosamente" : "Error al eliminar el préstamo";
                response.Data = result;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error interno del servidor";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }

        private async Task<List<string>> ValidateBusinessRulesForCreate(CreatePrestamoDto dto)
        {
            var errors = new List<string>();

            // Validar que la fecha del primer pago no sea requerida para tipo "Al Final"
            // Esto requeriría consultar el tipo de pago, pero por simplicidad lo omitimos por ahora

            // Validar fechas lógicas
            if (dto.FechaFinalPago.HasValue && dto.FechaPrimerPago.HasValue)
            {
                if (dto.FechaFinalPago <= dto.FechaPrimerPago)
                {
                    errors.Add("La fecha final de pago debe ser posterior a la fecha del primer pago");
                }
            }

            // Validar intereses coherentes
            if (dto.CantidadTotalAPagar < dto.CantidadTotalPrestada)
            {
                errors.Add("La cantidad total a pagar no puede ser menor a la cantidad prestada");
            }

            return errors;
        }

        private async Task<List<string>> ValidateBusinessRulesForUpdate(UpdatePrestamoDto dto)
        {
            var errors = new List<string>();

            // Validar fechas lógicas
            if (dto.FechaFinalPago.HasValue && dto.FechaPrimerPago.HasValue)
            {
                if (dto.FechaFinalPago <= dto.FechaPrimerPago)
                {
                    errors.Add("La fecha final de pago debe ser posterior a la fecha del primer pago");
                }
            }

            // Validar que los abonos no excedan las cantidades
            if (dto.TotalAbonadoCapital > dto.CantidadTotalPrestada)
            {
                errors.Add("El total abonado a capital no puede ser mayor a la cantidad prestada");
            }

            var totalAbonado = dto.TotalAbonadoCapital + dto.TotalAbonadoIntereses;
            if (totalAbonado > dto.CantidadTotalAPagar)
            {
                errors.Add("El total de abonos no puede exceder la cantidad total a pagar");
            }

            return errors;
        }

        public async Task<List<PrestamoHistorialDto>> GetHistorialPrestamoAsync(int prestamoId)
        {
            return await _prestamoRepository.GetHistorialPrestamoAsync(prestamoId);
        }
    }
}