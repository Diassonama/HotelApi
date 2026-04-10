using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Hotel.Application.DTOs.Request;
using Hotel.Application.DTOs.Response;
using Hotel.Domain.Identity;

//using Hotel.Application.Interfaces.Services;
using Hotel.Identity.Configurations;
//using Hotel.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Hotel.Identity.Services;

public class IdentityService   // : IIdentityService
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly JwtOptions _jwtOptions;

    public IdentityService(SignInManager<ApplicationUser> signInManager,
                           UserManager<ApplicationUser> userManager,
                           IOptions<JwtOptions> jwtOptions)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<UsuarioCadastroResponse> CadastrarUsuario(UsuarioCadastroRequest usuarioCadastro)
    {
        var ApplicationUser = new ApplicationUser
        {
            UserName = usuarioCadastro.Email,
            Email = usuarioCadastro.Email,
            EmailConfirmed = true
        };


        var result = await _userManager.CreateAsync(ApplicationUser, usuarioCadastro.Senha);
        if (result.Succeeded)
            await _userManager.SetLockoutEnabledAsync(ApplicationUser, false);

        var usuarioCadastroResponse = new UsuarioCadastroResponse(result.Succeeded);
        if (!result.Succeeded && result.Errors.Count() > 0)
            usuarioCadastroResponse.AdicionarErros(result.Errors.Select(r => r.Description));

        return usuarioCadastroResponse;
    }

    public async Task<UsuarioLoginResponse> Login(UsuarioLoginRequest usuarioLogin)
    {
        var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, true);
        if (result.Succeeded)
            return await GerarCredenciais(usuarioLogin.Email);

        var usuarioLoginResponse = new UsuarioLoginResponse();
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                usuarioLoginResponse.AdicionarErro("Essa conta está bloqueada");
            else if (result.IsNotAllowed)
                usuarioLoginResponse.AdicionarErro("Essa conta não tem permissão para fazer login");
            else if (result.RequiresTwoFactor)
                usuarioLoginResponse.AdicionarErro("É necessário confirmar o login no seu segundo fator de autenticação");
            else
                usuarioLoginResponse.AdicionarErro("Usuário ou senha estão incorretos");
        }

        return usuarioLoginResponse;
    }

    public async Task<UsuarioLoginResponse> LoginSemSenha(string usuarioId)
    {
        var usuarioLoginResponse = new UsuarioLoginResponse();
        var usuario = await _userManager.FindByIdAsync(usuarioId);

        if (await _userManager.IsLockedOutAsync(usuario))
            usuarioLoginResponse.AdicionarErro("Essa conta está bloqueada");
        else if (!await _userManager.IsEmailConfirmedAsync(usuario))
            usuarioLoginResponse.AdicionarErro("Essa conta precisa confirmar seu e-mail antes de realizar o login");

        if (usuarioLoginResponse.Sucesso)
            return await GerarCredenciais(usuario.Email);

        return usuarioLoginResponse;
    }

    private async Task<UsuarioLoginResponse> GerarCredenciais(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var accessTokenClaims = await ObterClaims(user, adicionarClaimsUsuario: true);
        var refreshTokenClaims = await ObterClaims(user, adicionarClaimsUsuario: false);

        var dataExpiracaoAccessToken = DateTime.Now.AddSeconds(_jwtOptions.AccessTokenExpiration);
        var dataExpiracaoRefreshToken = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpiration);
        var roles = accessTokenClaims.Where(c => c.Type == "role").Select(c => c.Value).ToList();

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

    private string GerarToken(IEnumerable<Claim> claims, DateTime dataExpiracao)
    {
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: dataExpiracao,
            signingCredentials: _jwtOptions.SigningCredentials);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private async Task<IList<Claim>> ObterClaims(ApplicationUser user, bool adicionarClaimsUsuario)
    {
        var claims = new List<Claim>();

        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));

        if (adicionarClaimsUsuario)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(userClaims);

            foreach (var role in roles)
                claims.Add(new Claim("role", role));
        }

        return claims;
    }
}
