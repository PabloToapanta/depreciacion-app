using AssetsService.Application.DTOs;
using AssetsService.Application.Interfaces;
using AssetsService.Application.Mappers;
using AssetsService.Domain.Entities;

namespace AssetsService.Application.UseCases;

// Orquesta el alta de un activo: valida que la categoria exista (via el
// puerto ICategoriaRepository), crea la entidad y la persiste (via
// IActivoRepository). Devuelve null si la categoria no existe.
public class CrearActivoUseCase
{
    private readonly IActivoRepository _activos;
    private readonly ICategoriaRepository _categorias;

    public CrearActivoUseCase(IActivoRepository activos, ICategoriaRepository categorias)
    {
        _activos = activos;
        _categorias = categorias;
    }

    public async Task<ActivoResponse?> EjecutarAsync(ActivoRequest request)
    {
        var categoria = await _categorias.ObtenerPorIdAsync(request.CategoriaId);
        if (categoria == null) return null;

        var activo = new Activo
        {
            CategoriaId = request.CategoriaId,
            Nombre = request.Nombre,
            PrecioCompra = request.PrecioCompra,
            FechaCompra = request.FechaCompra,
            FechaCorte = request.FechaCorte,
            FechaRegistro = DateTime.UtcNow
        };

        var creado = await _activos.CrearAsync(activo);
        return ActivoMapper.ToResponse(creado);
    }
}