using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class MenuRoleRepository : RepositoryBase<MenuRole>, IMenuRoleRepository
    {
        private readonly GhotelDbContext context;
        public MenuRoleRepository(GhotelDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<MenuRole> DeleteMenuAsync(string id)
        {
            var consulta = context.MenuRole.FirstOrDefault(m => m.RoleId == id);

            if (consulta == null)
            {
                return null;
            }

            var Remover = context.MenuRole.Remove(consulta);
            await context.SaveChangesAsync();
            return Remover.Entity;
        }



        public async Task<IEnumerable<MenuRole>> GetMenuAsync()
        {
            return await context.MenuRole
             .Include(m => m.Menu)
             .Include(m => m.Roles)
            .ToListAsync();
        }

        public Task<MenuRole> GetMenuByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MenuRole>> GetMenuItemByRoleAsync(string perfil)
        {
            return await context.MenuRole
                        .Include(m => m.Menu)
                        .Include(m => m.Roles)

                        .Where(m => m.RoleId == perfil)
                       .ToListAsync();
        }



        public async Task<MenuRole[]> InsertMenuAsync(MenuRole[] menu)
        {
            if (menu == null || menu.Length == 0)
                throw new ArgumentException("O array de menu não pode ser nulo ou vazio.", nameof(menu));

            var list = await context.MenuRole.Where(c => c.RoleId == menu[0].RoleId).ToListAsync();
            context.MenuRole.RemoveRange(list);

            var newMenuRoles = menu.Select(l => new MenuRole
            {
                MenuId = l.MenuId,
                RoleId = l.RoleId
            }).ToList();
            
            await context.MenuRole.AddRangeAsync(newMenuRoles);
            //await context.MenuRoles.AddAsync(menu);
            await context.SaveChangesAsync();
            return menu;
        }
    }
}