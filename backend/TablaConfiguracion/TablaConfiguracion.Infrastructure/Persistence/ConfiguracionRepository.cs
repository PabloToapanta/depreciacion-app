using Microsoft.EntityFrameworkCore;
using TablaConfiguracion.Application.Interfaces;
using TablaConfiguracion.Domain.Entities;

namespace TablaConfiguracion.Infrastructure.Persistence;

// Implementacion concreta del puerto IConfiguracionRepository usando EF Core.
public class ConfiguracionRepository : IConfiguracionRepository
{
    private readonly TablaConfiguracionDbContext _db;

    public ConfiguracionRepository(TablaConfiguracionDbContext db)
    {
        _db = db;
    }

    public async Task<Configuracion?> ObtenerAsync()
        => await _db.Configuracion
            .OrderBy(c => c.Id)
            .FirstOrDefaultAsync();
}