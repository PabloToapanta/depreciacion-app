namespace AuthService.DTOs;

// Respuesta del login: el JWT + datos basicos del usuario.
public class LoginResponse
{
    /// <summary>JWT firmado para autenticar las siguientes peticiones.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Nombre de usuario autenticado.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Nombre completo del usuario (si existe).</summary>
    public string? NombreCompleto { get; set; }

    /// <summary>Fecha y hora UTC en que expira el token.</summary>
    public DateTime ExpiraEn { get; set; }
}