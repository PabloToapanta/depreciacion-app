namespace AuthService.Application.Interfaces;

// Puerto de verificacion de contraseñas: Application no sabe (ni debe saber)
// que se usa BCrypt; Infrastructure lo implementa con BCrypt.Net-Next.
public interface IPasswordHasher
{
    bool Verificar(string password, string hash);
}