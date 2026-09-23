using AssetsService.Application.DTOs;
using AssetsService.Application.Interfaces;
using AssetsService.Domain.Services;

namespace AssetsService.Application.UseCases;

// Orquesta el calculo de la tabla de depreciacion: obtiene el activo con su
// categoria (via IActivoRepository) y delega el calculo a la regla de negocio
// pura DepreciacionService (Domain). Devuelve null si el activo no existe.
public class CalcularDepreciacionUseCase
{
    private readonly IActivoRepository _activos;
    private readonly DepreciacionService _depreciacion;

    public CalcularDepreciacionUseCase(IActivoRepository activos, DepreciacionService depreciacion)
    {
        _activos = activos;
        _depreciacion = depreciacion;
    }

    public async Task<TablaDepreciacionResponse?> EjecutarAsync(int id)
    {
        var activo = await _activos.ObtenerConCategoriaAsync(id);
        if (activo == null || activo.Categoria == null) return null;

        var filas = _depreciacion.CalcularTabla(activo, activo.Categoria);

        return new TablaDepreciacionResponse
        {
            ActivoId = activo.Id,
            Nombre = activo.Nombre,
            Categoria = activo.Categoria.Nombre,
            PrecioCompra = activo.PrecioCompra,
            FechaCompra = activo.FechaCompra,
            FechaCorte = activo.FechaCorte,
            Filas = filas
        };
    }
}