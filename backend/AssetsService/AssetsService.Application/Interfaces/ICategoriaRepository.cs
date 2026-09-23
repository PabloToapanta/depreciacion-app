using AssetsService.Domain.Entities;

namespace AssetsService.Application.Interfaces;

// Puerto de persistencia de categorias (el "maestro" del master-detail).
public interface ICategoriaRepository
{
    Task<Categoria?> ObtenerPorIdAsync(int id);
    Task<List<Categoria>> ListarAsync();
}