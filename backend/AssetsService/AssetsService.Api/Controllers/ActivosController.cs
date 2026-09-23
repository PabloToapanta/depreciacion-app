using AssetsService.Application.DTOs;
using AssetsService.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetsService.Api.Controllers;

// [Authorize] protege TODOS los endpoints de este controller:
// sin token valido el middleware responde 401 antes de ejecutar el codigo.
// El controller NO tiene logica: delega en los UseCases de Application.
[Authorize]
[ApiController]
[Route("api/activos")]
public class ActivosController : ControllerBase
{
    private readonly CrearActivoUseCase _crear;
    private readonly ListarActivosUseCase _listar;
    private readonly ObtenerActivoUseCase _obtener;
    private readonly CalcularDepreciacionUseCase _depreciacion;

    public ActivosController(
        CrearActivoUseCase crear,
        ListarActivosUseCase listar,
        ObtenerActivoUseCase obtener,
        CalcularDepreciacionUseCase depreciacion)
    {
        _crear = crear;
        _listar = listar;
        _obtener = obtener;
        _depreciacion = depreciacion;
    }

    /// <summary>
    /// Crea un activo nuevo.
    /// </summary>
    /// <param name="request">Datos del activo (categoria, nombre, precio, fechas).</param>
    /// <returns>El activo creado con su id.</returns>
    /// <response code="201">Activo creado.</response>
    /// <response code="400">La categoria indicada no existe.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ActivoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] ActivoRequest request)
    {
        var creado = await _crear.EjecutarAsync(request);
        if (creado == null)
        {
            return BadRequest(new { mensaje = $"La categoria {request.CategoriaId} no existe" });
        }

        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }

    /// <summary>
    /// Lista todos los activos registrados.
    /// </summary>
    /// <returns>Lista de activos (sin tabla de depreciacion).</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ActivoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
        => Ok(await _listar.EjecutarAsync());

    /// <summary>
    /// Obtiene un activo por su id.
    /// </summary>
    /// <param name="id">Id del activo.</param>
    /// <response code="200">Activo encontrado.</response>
    /// <response code="404">El activo no existe.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ActivoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obtener(int id)
    {
        var activo = await _obtener.EjecutarAsync(id);
        if (activo == null)
        {
            return NotFound(new { mensaje = $"El activo {id} no existe" });
        }

        return Ok(activo);
    }

    /// <summary>
    /// Calcula la tabla ANUAL de depreciacion del activo (con posible fila final parcial).
    /// </summary>
    /// <param name="id">Id del activo.</param>
    /// <returns>Datos del activo + filas anuales (Fecha | Depreciacion | Acumulado | Valor Real).</returns>
    /// <response code="200">Tabla calculada.</response>
    /// <response code="404">El activo no existe.</response>
    [HttpGet("{id}/depreciacion")]
    [ProducesResponseType(typeof(TablaDepreciacionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Depreciacion(int id)
    {
        var tabla = await _depreciacion.EjecutarAsync(id);
        if (tabla == null)
        {
            return NotFound(new { mensaje = $"El activo {id} no existe" });
        }

        return Ok(tabla);
    }
}