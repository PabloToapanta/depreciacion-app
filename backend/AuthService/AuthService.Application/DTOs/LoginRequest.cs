namespace AuthService.Application.DTOs;

// Cuerpo que el cliente envia en POST /api/auth/login.
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}