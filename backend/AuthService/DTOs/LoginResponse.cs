namespace AuthService.DTOs;

// Respuesta del login: el JWT + datos basicos del usuario.
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? NombreCompleto { get; set; }
    public DateTime ExpiraEn { get; set; }
}