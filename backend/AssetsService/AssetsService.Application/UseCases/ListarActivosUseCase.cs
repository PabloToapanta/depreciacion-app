using AssetsService.Application.DTOs;
using AssetsService.Application.Interfaces;
using AssetsService.Application.Mappers;

namespace AssetsService.Application.UseCases;

// Orquesta el listado de activos (mas recientes primero).
public class ListarActivosUseCase
{
    private readonly IActivoRepository _activos;

    public ListarActivosUseCase(IActivoRepository activos)
    {
        _activos = activos;
    }

    public async Task<List<ActivoResponse>> EjecutarAsync()
    {
        var activos = await _activos.ListarAsync();
        return activos.Select(ActivoMapper.ToResponse).ToList();
    }
}