using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TablaConfiguracion.Application.DTOs;
using TablaConfiguracion.Application.UseCases;

namespace TablaConfiguracion.Api.Controllers;

// Expone la configuracion del sistema (nombre, desarrolladores, version y fecha)
// para que el frontend la muestre en la tabla inferior.
[Authorize]
[ApiController]
[Route("api/configuracion")]
public class ConfiguracionController : ControllerBase
{
    private readonly ObtenerConfiguracionUseCase _obtener;

    public ConfiguracionController(ObtenerConfiguracionUseCase obtener)
    {
        _obtener = obtener;
    }

    /// <summary>
    /// Obtiene la configuracion del sistema.
    /// </summary>
    /// <returns>Nombre del sistema, nombres y apellidos, version y fecha.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ConfiguracionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obtener()
    {
        var configuracion = await _obtener.EjecutarAsync();
        if (configuracion is null)
            return NotFound();

        return Ok(configuracion);
    }
}