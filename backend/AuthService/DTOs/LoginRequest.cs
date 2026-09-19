namespace AuthService.DTOs;

// Cuerpo que el cliente envia en POST /api/auth/login.
public class LoginRequest
{
    /// <summary>Nombre de usuario registrado en AuthDB.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Contraseña en texto plano (se verifica contra el hash BCrypt).</summary>
    public string Password { get; set; } = string.Empty;
}