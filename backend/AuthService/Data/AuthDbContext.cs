using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

// DbContext = el "EntityManager" de EF Core: gestiona las conexiones
// y expone cada tabla como un DbSet consultable con LINQ.
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
}