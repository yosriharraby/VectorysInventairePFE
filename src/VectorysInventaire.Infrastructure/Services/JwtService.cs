using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;

namespace VectorysInventaire.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Utilisateur user)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var secret = jwtSection["Secret"] ?? throw new InvalidOperationException("Jwt:Secret manquant.");
        var issuer = jwtSection["Issuer"] ?? "VectorysInventaire";
        var audience = jwtSection["Audience"] ?? "VectorysInventaireClients";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = user.Role == RoleEnum.AGENT
            ? DateTime.UtcNow.AddHours(8)
            : DateTime.UtcNow.AddHours(24);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(ClaimTypes.Name, $"{user.Prenom} {user.Nom}".Trim()),
            new("nom", $"{user.Prenom} {user.Nom}".Trim()),
            new("codePersonnel", user.CodePersonnel ?? string.Empty)
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: expires,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
