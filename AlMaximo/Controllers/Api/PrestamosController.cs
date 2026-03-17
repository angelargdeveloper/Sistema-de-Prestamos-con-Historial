using Microsoft.AspNetCore.Mvc;
using AlMaximo.Core.DTOs;
using AlMaximo.Core.Interfaces;

namespace AlMaximo.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamosController : ControllerBase
    {
        private readonly IPrestamoService _prestamoService;

        public PrestamosController(IPrestamoService prestamoService)
        {
            _prestamoService = prestamoService;
        }

        /// <summary>
        /// Obtiene préstamos con paginación y búsqueda
        /// </summary>
        /// <param name="busqueda">Término de búsqueda (opcional)</param>
        /// <param name="empleadoId">ID del empleado para filtrar (opcional)</param>
        /// <param name="pageNumber">Número de página (por defecto 1)</param>
        /// <param name="pageSize">Tamaño de página (por defecto 20)</param>
        /// <returns>Lista paginada de préstamos</returns>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<PrestamoDto>>> GetPrestamos(
            [FromQuery] string? busqueda = null,
            [FromQuery] int? empleadoId = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var searchParameters = new SearchParametersDto
                {
                    Busqueda = busqueda,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _prestamoService.GetPrestamosAsync(searchParameters, empleadoId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obtiene un préstamo por ID
        /// </summary>
        /// <param name="id">ID del préstamo</param>
        /// <returns>Detalles del préstamo</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PrestamoDto>> GetPrestamo(int id)
        {
            try
            {
                var result = await _prestamoService.GetPrestamoByIdAsync(id);
                
                if (result == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Préstamo no encontrado"
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Crea un nuevo préstamo
        /// </summary>
        /// <param name="createPrestamoDto">Datos del préstamo a crear</param>
        /// <returns>ID del préstamo creado</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<int>>> CreatePrestamo([FromBody] CreatePrestamoDto createPrestamoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new ApiResponseDto<int>
                    {
                        Success = false,
                        Message = "Errores de validación",
                        Errors = errors
                    });
                }

                var result = await _prestamoService.CreatePrestamoAsync(createPrestamoDto);

                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetPrestamo), new { id = result.Data }, result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<int>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Actualiza un préstamo existente
        /// </summary>
        /// <param name="id">ID del préstamo</param>
        /// <param name="updatePrestamoDto">Datos actualizados del préstamo</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> UpdatePrestamo(int id, [FromBody] UpdatePrestamoDto updatePrestamoDto)
        {
            try
            {
                if (id != updatePrestamoDto.IdPrestamo)
                {
                    return BadRequest(new ApiResponseDto<bool>
                    {
                        Success = false,
                        Message = "El ID del préstamo no coincide"
                    });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new ApiResponseDto<bool>
                    {
                        Success = false,
                        Message = "Errores de validación",
                        Errors = errors
                    });
                }

                var result = await _prestamoService.UpdatePrestamoAsync(updatePrestamoDto);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Elimina un préstamo
        /// </summary>
        /// <param name="id">ID del préstamo</param>
        /// <param name="usuarioEliminacion">Usuario que realiza la eliminación</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<bool>>> DeletePrestamo(int id, [FromQuery] string usuarioEliminacion = "SYSTEM")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuarioEliminacion))
                {
                    return BadRequest(new ApiResponseDto<bool>
                    {
                        Success = false,
                        Message = "El usuario de eliminación es requerido"
                    });
                }

                var result = await _prestamoService.DeletePrestamoAsync(id, usuarioEliminacion);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<bool>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obtiene el historial de cambios de un préstamo específico
        /// </summary>
        /// <param name="id">ID del préstamo</param>
        /// <returns>Lista del historial de cambios</returns>
        [HttpGet("{id}/historial")]
        public async Task<ActionResult<List<PrestamoHistorialDto>>> GetHistorialPrestamo(int id)
        {
            try
            {
                var historial = await _prestamoService.GetHistorialPrestamoAsync(id);
                return Ok(historial);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Error interno del servidor",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}