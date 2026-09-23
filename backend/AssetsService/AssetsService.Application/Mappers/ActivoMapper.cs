using AssetsService.Application.DTOs;
using AssetsService.Domain.Entities;

namespace AssetsService.Application.Mappers;

// Mapeo Activo -> ActivoResponse (evita repetir el mismo codigo en los UseCases).
public static class ActivoMapper
{
    public static ActivoResponse ToResponse(Activo a) => new()
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