using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

// Puerto de persistencia de usuarios: Application dice QUE necesita
// (buscar por username) sin saber COMO se implementa (EF Core).
public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorUsernameAsync(string username);
}