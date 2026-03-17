using Microsoft.AspNetCore.Mvc;
using AlMaximo.Core.DTOs;
using AlMaximo.Core.Interfaces;

namespace AlMaximo.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogosController : ControllerBase
    {
        private readonly ICatalogosService _catalogosService;

        public CatalogosController(ICatalogosService catalogosService)
        {
            _catalogosService = catalogosService;
        }

        /// <summary>
        /// Obtiene empleados con paginación y búsqueda
        /// </summary>
        /// <param name="busqueda">Término de búsqueda (opcional)</param>
        /// <param name="pageNumber">Número de página (por defecto 1)</param>
        /// <param name="pageSize">Tamaño de página (por defecto 20)</param>
        /// <returns>Lista paginada de empleados</returns>
        [HttpGet("empleados")]
        public async Task<ActionResult<PagedResultDto<EmpleadoDto>>> GetEmpleados(
            [FromQuery] string? busqueda = null,
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

                var result = await _catalogosService.GetEmpleadosAsync(searchParameters);
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
        /// Obtiene la lista de empleados autorizadores
        /// </summary>
        /// <returns>Lista de empleados autorizadores</returns>
        [HttpGet("empleados-autorizadores")]
        public async Task<ActionResult<List<EmpleadoAutorizadorDto>>> GetEmpleadosAutorizadores()
        {
            try
            {
                var result = await _catalogosService.GetEmpleadosAutorizadoresAsync();
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
        /// Obtiene la lista de tipos de pago/abono
        /// </summary>
        /// <returns>Lista de tipos de pago</returns>
        [HttpGet("tipos-pago")]
        public async Task<ActionResult<List<TipoPagoAbonoDto>>> GetTiposPago()
        {
            try
            {
                var result = await _catalogosService.GetTiposPagoAbonoAsync();
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
    }
}