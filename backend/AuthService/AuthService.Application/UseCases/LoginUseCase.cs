using AuthService.Application.DTOs;
using AuthService.Application.Interfaces;

namespace AuthService.Application.UseCases;

// Orquesta el login: busca el usuario (IUsuarioRepository), verifica la
// contraseña (IPasswordHasher) y genera el JWT (IJwtService). Devuelve null
// si las credenciales son invalidas (mismo mensaje para no revelar si el
// usuario existe o no).
public class LoginUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwt;

    public LoginUseCase(IUsuarioRepository usuarios, IPasswordHasher passwordHasher, IJwtService jwt)
    {
        _usuarios = usuarios;
        _passwordHasher = passwordHasher;
        _jwt = jwt;
    }

    public async Task<LoginResponse?> EjecutarAsync(LoginRequest request)
    {
        var usuario = await _usuarios.ObtenerPorUsernameAsync(request.Username);
        if (usuario == null || !_passwordHasher.Verificar(request.Password, usuario.PasswordHash))
        {
            return null;
        }

        var (token, expiraEn) = _jwt.GenerarToken(usuario);

        return new LoginResponse
        {
            Token = token,
            Username = usuario.Username,
            NombreCompleto = usuario.NombreCompleto,
            ExpiraEn = expiraEn
        };
    }
}