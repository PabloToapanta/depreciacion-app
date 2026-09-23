using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Infrastructure.Services;

// Genera y firma el JWT con la clave secreta de appsettings.json.
// Implementa el puerto IJwtService de Application (inversion de dependencias):
// la firma HS256 es un detalle tecnico que vive en Infrastructure.
public class JwtService : IJwtService
{
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config)
    {
        _config = config;
    }

    public (string Token, DateTime ExpiraEn) GenerarToken(Usuario usuario)
    {
        // Clave secreta en bytes + algoritmo de firma (HS256)
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiraEn = DateTime.UtcNow.AddMinutes(
            _config.GetValue<int>("Jwt:ExpireMinutes"));

        // Datos que viajan dentro del token (claims)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("nombreCompleto", usuario.NombreCompleto ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiraEn,
            signingCredentials: creds);

        // WriteToken serializa el token al string "eyJhbGciOi..."
        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEn);
    }
}