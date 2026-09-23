using Microsoft.EntityFrameworkCore;
using TablaConfiguracion.Domain.Entities;

namespace TablaConfiguracion.Infrastructure.Persistence;

// DbContext de TablaConfiguracion: expone la tabla `configuracion`.
// Vive en Infrastructure porque es un detalle de persistencia (EF Core).
public class TablaConfiguracionDbContext : DbContext
{
    public TablaConfiguracionDbContext(DbContextOptions<TablaConfiguracionDbContext> options)
        : base(options) { }

    public DbSet<Configuracion> Configuracion => Set<Configuracion>();
}