using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence;

// DbContext = el "EntityManager" de EF Core: gestiona las conexiones
// y expone cada tabla como un DbSet consultable con LINQ.
// Vive en Infrastructure porque es un detalle de persistencia (EF Core).
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
}