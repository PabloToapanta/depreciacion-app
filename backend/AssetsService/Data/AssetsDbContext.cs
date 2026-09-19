using AssetsService.Models;
using Microsoft.EntityFrameworkCore;

namespace AssetsService.Data;

// DbContext de AssetsService: expone las tablas Categorias y Activos.
public class AssetsDbContext : DbContext
{
    public AssetsDbContext(DbContextOptions<AssetsDbContext> options) : base(options) { }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Activo> Activos => Set<Activo>();
}