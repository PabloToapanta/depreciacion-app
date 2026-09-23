using AssetsService.Application.DTOs;
using AssetsService.Application.Interfaces;
using AssetsService.Application.Mappers;

namespace AssetsService.Application.UseCases;

// Orquesta la consulta de un activo por id. Devuelve null si no existe.
public class ObtenerActivoUseCase
{
    private readonly IActivoRepository _activos;

    public ObtenerActivoUseCase(IActivoRepository activos)
    {
        _activos = activos;
    }

    public async Task<ActivoResponse?> EjecutarAsync(int id)
    {
        var activo = await _activos.ObtenerConCategoriaAsync(id);
        return activo == null ? null : ActivoMapper.ToResponse(activo);
    }
}