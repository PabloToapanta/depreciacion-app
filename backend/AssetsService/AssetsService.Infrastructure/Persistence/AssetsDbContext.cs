using AssetsService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetsService.Infrastructure.Persistence;

// DbContext de AssetsService: expone las tablas Categorias y Activos.
// Vive en Infrastructure porque es un detalle de persistencia (EF Core);
// ni Domain ni Application lo conocen.
public class AssetsDbContext : DbContext
{
    public AssetsDbContext(DbContextOptions<AssetsDbContext> options) : base(options) { }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Activo> Activos => Set<Activo>();
}