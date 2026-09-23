using AssetsService.Application.DTOs;
using AssetsService.Application.Interfaces;

namespace AssetsService.Application.UseCases;

// Orquesta el listado de categorias (para el select del formulario).
public class ListarCategoriasUseCase
{
    private readonly ICategoriaRepository _categorias;

    public ListarCategoriasUseCase(ICategoriaRepository categorias)
    {
        _categorias = categorias;
    }

    public async Task<List<CategoriaResponse>> EjecutarAsync()
    {
        var categorias = await _categorias.ListarAsync();
        return categorias.Select(c => new CategoriaResponse
        {
            Id = c.Id,
            Nombre = c.Nombre,
            VidaUtilAnios = c.VidaUtilAnios
        }).ToList();
    }
}