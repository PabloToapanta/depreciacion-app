using AssetsService.Data;
using AssetsService.DTOs;
using AssetsService.Models;
using AssetsService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetsService.Controllers;

// [Authorize] protege TODOS los endpoints de este controller:
// sin token valido el middleware responde 401 antes de ejecutar el codigo.
[Authorize]
[ApiController]
[Route("api/activos")]
public class ActivosController : ControllerBase
{
    private readonly AssetsDbContext _db;
    private readonly DepreciacionService _depreciacion;

    public ActivosController(AssetsDbContext db, DepreciacionService depreciacion)
    {
        _db = db;
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
        // Validar que la categoria exista (la vida util se deduce de ella)
        var categoria = await _db.Categorias.FindAsync(request.CategoriaId);
        if (categoria == null)
        {
            return BadRequest(new { mensaje = $"La categoria {request.CategoriaId} no existe" });
        }

        var activo = new Activo
        {
            CategoriaId = request.CategoriaId,
            Nombre = request.Nombre,
            PrecioCompra = request.PrecioCompra,
            FechaCompra = request.FechaCompra,
            FechaCorte = request.FechaCorte,
            FechaRegistro = DateTime.UtcNow
        };

        _db.Activos.Add(activo);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obtener), new { id = activo.Id }, Mapear(activo));
    }

    /// <summary>
    /// Lista todos los activos registrados.
    /// </summary>
    /// <returns>Lista de activos (sin tabla de depreciacion).</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ActivoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var activos = await _db.Activos
            .Include(a => a.Categoria)
            .OrderByDescending(a => a.FechaRegistro)
            .ToListAsync();

        return Ok(activos.Select(Mapear));
    }

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
        var activo = await _db.Activos
            .Include(a => a.Categoria)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (activo == null)
        {
            return NotFound(new { mensaje = $"El activo {id} no existe" });
        }

        return Ok(Mapear(activo));
    }

    /// <summary>
    /// Calcula la tabla mensual de depreciacion del activo.
    /// </summary>
    /// <param name="id">Id del activo.</param>
    /// <returns>Datos del activo + filas mensuales (Fecha | Depreciacion | Acumulado | Valor Real).</returns>
    /// <response code="200">Tabla calculada.</response>
    /// <response code="404">El activo no existe.</response>
    [HttpGet("{id}/depreciacion")]
    [ProducesResponseType(typeof(TablaDepreciacionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Depreciacion(int id)
    {
        var activo = await _db.Activos
            .Include(a => a.Categoria)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (activo == null || activo.Categoria == null)
        {
            return NotFound(new { mensaje = $"El activo {id} no existe" });
        }

        var filas = _depreciacion.CalcularTabla(activo, activo.Categoria);

        var response = new TablaDepreciacionResponse
        {
            ActivoId = activo.Id,
            Nombre = activo.Nombre,
            Categoria = activo.Categoria.Nombre,
            PrecioCompra = activo.PrecioCompra,
            FechaCompra = activo.FechaCompra,
            FechaCorte = activo.FechaCorte,
            Filas = filas
        };

        return Ok(response);
    }

    // Mapeo Activo -> ActivoResponse (evita repetir el mismo codigo en 3 endpoints)
    private static ActivoResponse Mapear(Activo a) => new()
    {
        Id = a.Id,
        CategoriaId = a.CategoriaId,
        CategoriaNombre = a.Categoria?.Nombre ?? string.Empty,
        Nombre = a.Nombre,
        PrecioCompra = a.PrecioCompra,
        FechaCompra = a.FechaCompra,
        FechaCorte = a.FechaCorte
    };
}