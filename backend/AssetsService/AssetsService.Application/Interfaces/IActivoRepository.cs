using AssetsService.Domain.Entities;

namespace AssetsService.Application.Interfaces;

// Puerto de persistencia de activos: Application dice QUE necesita
// (obtener con categoria, listar, crear) sin saber COMO se implementa.
// Infrastructure lo implementa con EF Core (inversion de dependencias).
public interface IActivoRepository
{
    Task<Activo?> ObtenerConCategoriaAsync(int id);
    Task<List<Activo>> ListarAsync();
    Task<Activo> CrearAsync(Activo activo);
}