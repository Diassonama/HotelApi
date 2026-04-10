using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hotel.Infrastruture.Persistence.Shared
{
    public class UsuarioLogado
    {
      private readonly IHttpContextAccessor _accessor;

        public UsuarioLogado(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string Email => GetClaimsIdentity().FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;  //_accessor.HttpContext.User.Identity.Name;
        public string IdUtilizador => GetClaimsIdentity().FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value;
        public string Usuario => GetClaimsIdentity().FirstOrDefault(a => a.Type == ClaimTypes.GivenName)?.Value;
        public string Utilizador => GetClaimsIdentity().FirstOrDefault(a => a.Type == ClaimTypes.Name)?.Value;
        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }  
    }
}