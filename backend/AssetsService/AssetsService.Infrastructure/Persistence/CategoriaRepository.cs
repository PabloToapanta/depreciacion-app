using AssetsService.Application.Interfaces;
using AssetsService.Domain.Entities;
using AssetsService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetsService.Infrastructure.Persistence;

// Implementacion concreta del puerto ICategoriaRepository usando EF Core.
public class CategoriaRepository : ICategoriaRepository
{
    private readonly AssetsDbContext _db;

    public CategoriaRepository(AssetsDbContext db)
    {
        _db = db;
    }

    public async Task<Categoria?> ObtenerPorIdAsync(int id)
        => await _db.Categorias.FindAsync(id);

    public async Task<List<Categoria>> ListarAsync()
        => await _db.Categorias
            .OrderBy(c => c.Id)
            .ToListAsync();
}