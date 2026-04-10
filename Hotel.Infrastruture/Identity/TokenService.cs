using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hotel.Application.Common;
using Hotel.Application.Contracts;
using Hotel.Application.DTOs.Response;
using Hotel.Application.Interfaces;
using Hotel.Domain.Entities;
using Hotel.Domain.Identity;
//using Hotel.Infrastruture.Migrations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Hotel.Infrastruture.Identity
{
    public class TokenService : ITokenService
    {
        public readonly UserManager<ApplicationUser> _userManager;
        //   private readonly SignInManager<ApplicationUser> _signInManager;
        //  private readonly RoleManager<IdentityRole> _roleManager;
        public readonly JwtSettings _jwtSettings;
        public readonly IClaimService _claimService;
        private readonly IpAddressService _ipAddressService;
        //   public readonly ITokenService _tokenService;

        public TokenService(UserManager<ApplicationUser> userManager,
         IOptions<JwtSettings> jwtSettings, IClaimService claimService, IpAddressService ipAddressService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            //    _signInManager = signInManager;
            //    _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _claimService = claimService;
            _ipAddressService = ipAddressService;

            //   _tokenService = tokenService;
            if (_jwtSettings.SigningCredentials == null)
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
                _jwtSettings.SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            }
        }

        public RefreshToken GenerateRefreshToken(string token)
        {
            return new RefreshToken
            {
                Token = token,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = _ipAddressService.GetIpAddress()
            };
        }

        public string GenerateToken(string user_id, string username, string province)
        {
            throw new NotImplementedException();
        }

        public Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public string GenerateToken(string user_id, string username, string province, bool? mustConfirm)
        {
            throw new NotImplementedException();
        }

       /*  public async Task<UsuarioLoginResponse> GerarCredenciais(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new ArgumentException("Usuário não encontrado com o email fornecido.");

            if (!user.EmailConfirmed)
                throw new InvalidOperationException("Email não confirmado. O usuário não pode fazer login.");
            // var roless = await _userManager.GetRolesAsync(user);
            var accessTokenClaims = await _claimService.ObterClaims(user, adicionarClaimsUsuario: true);
            var refreshTokenClaims = await _claimService.ObterClaims(user, adicionarClaimsUsuario: false);
            // var role = await _claimService.ObterClaims(user, adicionarClaimsUsuario: true);
            //var claims = await _claimService.ObterClaims(user, adicionarClaimsUsuario: true);
            var roles = accessTokenClaims.Where(c => c.Type == "role").Select(c => c.Value).ToList();


            var dataExpiracaoAccessToken = DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpiration);
            var dataExpiracaoRefreshToken = DateTime.Now.AddSeconds(_jwtSettings.RefreshTokenExpiration);

            var accessToken = GerarToken(accessTokenClaims, dataExpiracaoAccessToken);
            var refreshToken = GerarToken(refreshTokenClaims, dataExpiracaoRefreshToken);

            return new UsuarioLoginResponse
            (
                sucesso: true,
                accessToken: accessToken,
                refreshToken: refreshToken,
                usuario: user.UserName,
                role: roles.FirstOrDefault(),
                id: user.Id
            );
        }
         */
        
        public async Task<UsuarioLoginResponse> GerarCredenciais(string email)
{
    var user = await _userManager.FindByEmailAsync(email)
        ?? throw new ArgumentException("Usuário não encontrado com o email fornecido.");

    if (!user.EmailConfirmed)
        throw new InvalidOperationException("Email não confirmado. O usuário não pode fazer login.");

    // Obtém claims
    var accessTokenClaims = await _claimService.ObterClaims(user, adicionarClaimsUsuario: true);
    var refreshTokenClaims = await _claimService.ObterClaims(user, adicionarClaimsUsuario: false);

    // Define tempos de expiração
    var accessTokenExpiracao = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiration);
    var refreshTokenExpiracao = DateTime.UtcNow.AddSeconds(_jwtSettings.RefreshTokenExpiration);

    // Gera os tokens
    var accessToken = GerarToken(accessTokenClaims, accessTokenExpiracao);
    var refreshToken = GerarToken(refreshTokenClaims, refreshTokenExpiracao);

    // Cria a resposta
    return new UsuarioLoginResponse(
        sucesso: true,
        accessToken: accessToken,
        refreshToken: refreshToken,
        usuario: user.UserName,
        role: accessTokenClaims.FirstOrDefault(c => c.Type == "role")?.Value,
        id: user.Id
    );
}

        
    /*     public string GerarToken(IEnumerable<Claim> claims, DateTime dataExpiracao)
        {
            if (claims == null || !claims.Any())
                throw new ArgumentException("Nenhuma claim fornecida para gerar o token.");

            if (_jwtSettings.SigningCredentials == null)
                throw new InvalidOperationException("As credenciais de assinatura não estão configuradas.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));


            var jwt = new JwtSecurityToken(
              issuer: _jwtSettings.Issuer,
              audience: _jwtSettings.Audience,
              claims: claims,
              notBefore: DateTime.UtcNow,
              expires: dataExpiracao,
              signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)); //_jwtSettings.SigningCredentials

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
 */

public string GerarToken(IEnumerable<Claim> claims, DateTime dataExpiracao)
{
    if (claims == null || !claims.Any())
        throw new ArgumentException("Nenhuma claim fornecida para gerar o token.");

    if (dataExpiracao <= DateTime.UtcNow)
        throw new ArgumentException("A data de expiração do token deve ser maior que o momento atual.");

    if (_jwtSettings.SigningCredentials == null)
        throw new InvalidOperationException("As credenciais de assinatura não estão configuradas.");

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

    var jwt = new JwtSecurityToken(
        issuer: _jwtSettings.Issuer,
        audience: _jwtSettings.Audience,
        claims: claims,
        notBefore: DateTime.UtcNow,
        expires: dataExpiracao,
        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

    return new JwtSecurityTokenHandler().WriteToken(jwt);
}


        public string RefreshToken(string token, string ipAddress)
        {
            /*         var storedToken = _refreshTokenRepository.GetByToken(token);
            if (storedToken == null || !storedToken.IsActive)
                throw new SecurityTokenException("Token inválido ou expirado.");

            var newToken = GenerateRefreshToken(storedToken.Token);
            _refreshTokenRepository.Update(storedToken, newToken);
         */
            return ""; //newToken.Token;
        }

        public void RevokeToken(string token)
        {
            throw new NotImplementedException();
        }
    }


}
