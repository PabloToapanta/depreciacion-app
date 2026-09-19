using AuthService.Data;
using AuthService.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _db;

    public AuthController(AuthDbContext db)
    {
        _db = db;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // 1. Buscar el usuario por nombre de usuario (LINQ ~ JPQL)
        var usuario = await _db.Usuarios
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        // 2. Si no existe, o la clave no coincide -> 401 (mismo mensaje para
        //    no revelar si el usuario existe o no)
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
        {
            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });
        }

        // 3. Credenciales validas -> 200 (el JWT se agrega en el siguiente paso)
        var response = new LoginResponse
        {
            Token = string.Empty, // TODO: paso 4 - generar y firmar JWT
            Username = usuario.Username,
            NombreCompleto = usuario.NombreCompleto,
            ExpiraEn = DateTime.UtcNow
        };

        return Ok(response);
    }
}