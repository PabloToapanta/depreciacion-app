using AuthService.Application.Interfaces;

namespace AuthService.Infrastructure.Services;

// Implementa el puerto IPasswordHasher con BCrypt.Net-Next.
// El algoritmo de hash es un detalle tecnico que vive en Infrastructure;
// Application solo sabe que existe un "verificador de contraseñas".
public class BCryptPasswordHasher : IPasswordHasher
{
    public bool Verificar(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}