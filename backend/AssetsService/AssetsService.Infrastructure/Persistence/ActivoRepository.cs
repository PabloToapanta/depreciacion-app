using AssetsService.Application.Interfaces;
using AssetsService.Domain.Entities;
using AssetsService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssetsService.Infrastructure.Persistence;

// Implementacion concreta del puerto IActivoRepository usando EF Core.
// Esta es la inversion de dependencias: Application define el contrato,
// Infrastructure decide el motor de persistencia.
public class ActivoRepository : IActivoRepository
{
    private readonly AssetsDbContext _db;

    public ActivoRepository(AssetsDbContext db)
    {
        _db = db;
    }

    public async Task<Activo?> ObtenerConCategoriaAsync(int id)
        => await _db.Activos
            .Include(a => a.Categoria)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<List<Activo>> ListarAsync()
        => await _db.Activos
            .Include(a => a.Categoria)
            .OrderByDescending(a => a.FechaRegistro)
            .ToListAsync();

    public async Task<Activo> CrearAsync(Activo activo)
    {
        _db.Activos.Add(activo);
        await _db.SaveChangesAsync();
        return activo;
    }
}