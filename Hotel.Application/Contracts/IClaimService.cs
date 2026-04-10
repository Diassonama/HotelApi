using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Hotel.Application.Contracts
{
    public interface IClaimService
    {
        Task<IList<Claim>> ObterClaims(Domain.Identity.ApplicationUser user, bool adicionarClaimsUsuario);
      
    }
}