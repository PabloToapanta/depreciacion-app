using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Persistence;

// Implementacion concreta del puerto IUsuarioRepository usando EF Core.
// Inversion de dependencias: Application define el contrato, Infrastructure
// decide el motor de persistencia.
public class UsuarioRepository : IUsuarioRepository
{
    private readonly AuthDbContext _db;

    public UsuarioRepository(AuthDbContext db)
    {
        _db = db;
    }

    public async Task<Usuario?> ObtenerPorUsernameAsync(string username)
        => await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Username == username);
}