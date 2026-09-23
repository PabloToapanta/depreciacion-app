using AuthService.Application.DTOs;
using AuthService.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

// El controller NO tiene logica: delega en el LoginUseCase de Application.
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _login;

    public AuthController(LoginUseCase login)
    {
        _login = login;
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
        var response = await _login.EjecutarAsync(request);
        if (response == null)
        {
            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });
        }

        return Ok(response);
    }
}