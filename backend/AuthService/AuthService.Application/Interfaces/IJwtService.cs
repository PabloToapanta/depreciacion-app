using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

// Puerto de generacion de JWT: Application define el contrato, Infrastructure
// lo implementa con Microsoft.IdentityModel (firma HS256).
public interface IJwtService
{
    (string Token, DateTime ExpiraEn) GenerarToken(Usuario usuario);
}