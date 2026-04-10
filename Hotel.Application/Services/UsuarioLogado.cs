using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;

namespace Hotel.Application.Services
{
    public class UsuarioLogado : IUsuarioLogado
    {
         private readonly IHttpContextAccessor _accessor;
                 
        public UsuarioLogado(IHttpContextAccessor accessor)
        {
            _accessor = accessor;

        }

    //busca o role do usuario logado
        public string Role => _accessor.HttpContext?.User?.FindFirst(c => c.Type == ClaimTypes.Role)?.Value;
      public ClaimsPrincipal User => _accessor.HttpContext?.User;
    
        public string UserId => _accessor.HttpContext?.User?.Identity?.Name;
     //    public string subClaim => _accessor.HttpContext.User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Sub);

         public string userId2 => GetClaimsIdentity().FirstOrDefault(a => a.Type == ClaimTypes.Name)?.Value;
       // public string userId2 => _accessor.HttpContext?.User?.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        public string Email => GetClaimsIdentity().FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;  //_accessor.HttpContext.User.Identity.Name;
        
      // public string GetUserId => user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string IdUtilizador => GetClaimsIdentity().FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value;
        public string UserName => GetClaimsIdentity().FirstOrDefault(a => a.Type == ClaimTypes.GivenName)?.Value;
        public string Utilizador => GetClaimsIdentity().FirstOrDefault(a => a.Type == ClaimTypes.Name)?.Value; 
        public string perfil => GetClaimsIdentity().FirstOrDefault(a => a.Type == ClaimTypes.Role)?.Value;
       
        public string Usuario => User.GetUserName();
        public string UsuarioId => User.GetUserId();
        public IEnumerable<Claim> GetClaimsIdentity()
        {
            return _accessor.HttpContext.User.Claims;
        }  
    }
}