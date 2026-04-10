using System;
using System.Collections.Generic;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.DTOs.Response;
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class UtilizadorRepository : RepositoryBase<Utilizador>, IUtilizadorRepository
    {
        private  readonly GhotelDbContext _context;
        public UtilizadorRepository(GhotelDbContext dbContext, GhotelDbContext context) : base(dbContext)
        {
            _context = context;
        }

        /*  private readonly GhotelDbContext _context;
public UtilizadorRepository(GhotelDbContext Context) : base(Context)
{
    _context = Context;
} */

        public async Task<IEnumerable<Utilizador>> usuarios()
        {
            var utilizadoresComRoles = await (from utilizador in _context.Utilizadores
                                             join userRole in _context.UserRoles on utilizador.Id equals userRole.UserId into userRoleGroup
                                             from userRole in userRoleGroup.DefaultIfEmpty()
                                             join role in _context.Roles on userRole.RoleId equals role.Id into roleGroup
                                             from role in roleGroup.DefaultIfEmpty()
                                             select new
                                             {
                                                 Utilizador = utilizador,
                                                 Role = role
                                             }).ToListAsync();

            // Agrupar por utilizador e criar a lista de roles para cada um
            var resultado = utilizadoresComRoles
                .GroupBy(x => x.Utilizador.Id)
                .Select(group => 
                {
                    var utilizador = group.First().Utilizador;
                    
                    // Criar uma nova instância de Utilizador com as propriedades necessárias
                    var utilizadorResult = new Utilizador
                    {
                        Id = utilizador.Id,
                        UserName = utilizador.UserName,
                        Email = utilizador.Email,
                        FirstName = utilizador.FirstName,
                        LastName = utilizador.LastName,
                        PhoneNumber = utilizador.PhoneNumber,
                        EmailConfirmed = utilizador.EmailConfirmed,
                        PhoneNumberConfirmed = utilizador.PhoneNumberConfirmed,
                        TwoFactorEnabled = utilizador.TwoFactorEnabled,
                        LockoutEnd = utilizador.LockoutEnd,
                        LockoutEnabled = utilizador.LockoutEnabled,
                        AccessFailedCount = utilizador.AccessFailedCount,
                        // Adicionar roles como uma propriedade customizada (se necessário)
                        // Roles = group.Where(x => x.Role != null).Select(x => x.Role.Name).ToList()
                    };
                    
                    return utilizadorResult;
                })
                .ToList();

            return resultado;
        }

        public async Task<IEnumerable<UsuariosResponse>> UsuariosComRoles()
        {
            var utilizadoresComRoles = await (from utilizador in _context.Utilizadores
                                             join userRole in _context.UserRoles on utilizador.Id equals userRole.UserId into userRoleGroup
                                             from userRole in userRoleGroup.DefaultIfEmpty()
                                             join role in _context.Roles on userRole.RoleId equals role.Id into roleGroup  
                                             from role in roleGroup.DefaultIfEmpty()
                                             select new
                                             {
                                                 UtilizadorId = utilizador.Id,
                                                 utilizador.UserName,
                                                 utilizador.Email,
                                                 utilizador.FirstName,
                                                 utilizador.LastName,
                                                 RoleId = role != null ? role.Id : null,
                                                 RoleName = role != null ? role.Name : null
                                             }).ToListAsync();

            // Agrupar por utilizador e criar UsuariosResponse com lista de roles
            var resultado = utilizadoresComRoles
                .GroupBy(x => new { x.UtilizadorId, x.UserName, x.Email, x.FirstName, x.LastName })
                .Select(group => new UsuariosResponse
                {
                    Id = group.Key.UtilizadorId,
                    UserName = group.Key.UserName,
                    Email = group.Key.Email,
                    FirstName = group.Key.FirstName,
                    LastName = group.Key.LastName,
                    UserRoles = group.Where(x => x.RoleId != null)
                                   .Select(x => new RoleResponse
                                   {
                                       Id = x.RoleId,
                                       Name = x.RoleName
                                   }).ToList()
                })
                .ToList();

            return resultado;
        }

public async Task<string> GetNomeCompletoByIdAsync(string id)
{
    if (string.IsNullOrWhiteSpace(id))
        return "Sistema";

    try
    {
        var utilizador = await _context.Utilizadores
            .AsNoTracking()
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync();

        if (utilizador == null)
            return "Sistema";

        var nomeCompleto = $"{utilizador.FirstName} {utilizador.LastName}".Trim();
        return string.IsNullOrWhiteSpace(nomeCompleto) ? "Sistema" : nomeCompleto;
    }
    catch
    {
        return "Sistema";
    }
}

        public async Task<UsuariosResponse> GetByIdAsync(string id)
{
    if (string.IsNullOrWhiteSpace(id))
    {
        return null;
    }

    return
   /*   await _context.Utilizadores
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == id); */


        await (from utilizador in _context.Utilizadores
                                             join userRole in _context.UserRoles on utilizador.Id equals userRole.UserId into userRoleGroup
                                             from userRole in userRoleGroup.DefaultIfEmpty()
                                             join role in _context.Roles on userRole.RoleId equals role.Id into roleGroup  
                                             from role in roleGroup.DefaultIfEmpty()
                                             where utilizador.Id == id
                                             select new
                                             {
                                                 UtilizadorId = utilizador.Id,
                                                 utilizador.UserName,
                                                 utilizador.Email,
                                                 utilizador.FirstName,
                                                 utilizador.LastName,
                                                 RoleId = role != null ? role.Id : null,
                                                 RoleName = role != null ? role.Name : null
                                             }).ToListAsync().ContinueWith(t =>
        {
            var data = t.Result;
            if (!data.Any())
                return null;

            var first = data.First();
            return new UsuariosResponse
            {
                Id = first.UtilizadorId,
                UserName = first.UserName,
                Email = first.Email,
                FirstName = first.FirstName,
                LastName = first.LastName,
                UserRoles = data.Where(x => x.RoleId != null)
                               .Select(x => new RoleResponse
                               {
                                   Id = x.RoleId,
                                   Name = x.RoleName
                               }).ToList()
            };
        });
}
    }
}