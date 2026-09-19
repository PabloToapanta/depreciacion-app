using AuthService.Data;
using AuthService.DTOs;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _db;
    private readonly JwtService _jwt;

    public AuthController(AuthDbContext db, JwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    /// <summary>
    /// Inicia sesion con usuario y contraseña.
    /// </summary>
    /// <param name="request">Credenciales del usuario (username y password).</param>
    /// <returns>JWT firmado si las credenciales son validas.</returns>
    /// <response code="200">Credenciales validas: devuelve el JWT.</response>
    /// <response code="401">Usuario o contraseña incorrectos.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

        // 3. Credenciales validas -> generar JWT y responder 200
        var (token, expiraEn) = _jwt.GenerarToken(usuario);

        var response = new LoginResponse
        {
            Token = token,
            Username = usuario.Username,
            NombreCompleto = usuario.NombreCompleto,
            ExpiraEn = expiraEn
        };

        return Ok(response);
    }
}