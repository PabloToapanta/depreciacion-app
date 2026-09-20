using AssetsService.Data;
using AssetsService.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetsService.Controllers;

// Lista las categorias (Tecnología, Vehículos, Edificios, Muebles) para que
// el frontend llene el select del formulario sin hardcodear ids.
[Authorize]
[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly AssetsDbContext _db;

    public CategoriasController(AssetsDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Lista todas las categorias del sistema.
    /// </summary>
    /// <returns>Lista de categorias (id, nombre, vida util en años).</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoriaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var categorias = await _db.Categorias
            .OrderBy(c => c.Id)
            .ToListAsync();

        return Ok(categorias.Select(c => new CategoriaResponse
        {
            Id = c.Id,
            Nombre = c.Nombre,
            VidaUtilAnios = c.VidaUtilAnios
        }));
    }
}