
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Hotel.Application.DTOs.Response;
using Hotel.Domain.Entities;
using Hotel.Domain.Identity;


namespace Hotel.Application.Common
{
    public interface ITokenService
    {
        string RefreshToken(string token, string ipAddress);
        string GerarToken(IEnumerable<Claim> claims, DateTime dataExpiracao);

        string GenerateToken(string user_id, string username, string province);
        Task<JwtSecurityToken> GenerateToken(ApplicationUser user);

        string GenerateToken(string user_id, string username, string province, bool? mustConfirm); 
        Task<UsuarioLoginResponse> GerarCredenciais(string email);

        RefreshToken GenerateRefreshToken(string token);

    //    void RevokeToken(string token);
    }
}