using AssetsService.Application.DTOs;
using AssetsService.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetsService.Api.Controllers;

// Lista las categorias (Tecnología, Vehículos, Edificios, Muebles) para que
// el frontend llene el select del formulario sin hardcodear ids.
[Authorize]
[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly ListarCategoriasUseCase _listar;

    public CategoriasController(ListarCategoriasUseCase listar)
    {
        _listar = listar;
    }

    /// <summary>
    /// Lista todas las categorias del sistema.
    /// </summary>
    /// <returns>Lista de categorias (id, nombre, vida util en años).</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoriaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
        => Ok(await _listar.EjecutarAsync());
}