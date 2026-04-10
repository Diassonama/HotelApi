using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hotel.Application.Contracts;
using Hotel.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Infrastruture.Identity
{
    public class ClaimService: IClaimService
    {
      private readonly UserManager<ApplicationUser> _userManager;

        public ClaimService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IList<Claim>> ObterClaims(ApplicationUser user, bool adicionarClaimsUsuario)
        {
             var claims = new List<Claim>
             {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                 new Claim(ClaimTypes.NameIdentifier, user.Id),
                 new Claim("id", user.Id),
                 new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                 new Claim(JwtRegisteredClaimNames.Email, user.Email),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                 new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
                 
             };

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
}