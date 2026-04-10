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
    public class MenuRepository : RepositoryBase<AppMenu>, IMenuRepository
    {
        private readonly GhotelDbContext _context;
        public MenuRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<AppMenu> DeleteMenuAsync(int id)
        {
            var consulta = await _context.AppMenu.FindAsync(id);

            if (consulta == null)
            {
                return null;
            }

            var Remover = _context.AppMenu.Remove(consulta);
            await _context.SaveChangesAsync();
            return Remover.Entity;
        }

        public async Task<IEnumerable<AppMenu>> GetMenuAsync()
        {
            return await _context.AppMenu
                        //     .Include(m => m.MenuItems).ThenInclude(m => m.Perfis)
                        .ToListAsync();
        }
        public async Task<IEnumerable<MenuAcesso>> GetMenuAcessobyRoleAsync(string id)
        {
            var menu = await _context.AppMenu
                         .Include(m => m.MenuRole).ThenInclude(m => m.Roles)
                         .Select(x => new MenuAcesso
                         {
                             IdMenu = x.Id,
                             Nome = x.Nome,
                             check = x.MenuRole.Count(x => x.Roles.Id == id) > 0 ? true : false
                         })
                        .ToListAsync();

            return menu;
        }

        public async Task<IEnumerable<MenuRole>> GetMenuByRoleAsync(string perfil)
        {
            return await _context.MenuRole
                         .Include(m => m.Menu)
                         .Include(m => m.Roles)

                         .Where(m => m.Roles.Name.ToUpper() == perfil.ToUpper())
                        .ToListAsync();
        }

        public async Task<bool> HaveAcess(string path, string Perfil)
        {
            var consulta = await _context.MenuRole
                                    .Include(m => m.Menu)
                                    .Include(m => m.Roles)
                                    .FirstOrDefaultAsync(m => m.Menu.Path.ToLower() == path.ToLower() && m.Roles.Name.ToLower() == Perfil.ToLower());

            if (consulta == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public IQueryable<AppMenu> GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            var query = _context.AppMenu
                        .Include(p => p.MenuRole)
                        .AsNoTracking();

            if (paginationFilter?.FieldFilter is not null && !string.IsNullOrWhiteSpace(paginationFilter.FieldFilter))
            {
                var fieldFilter = paginationFilter.FieldFilter.ToLower();
                query = query.Where(r => r.Nome.ToString().Contains(fieldFilter));
            }

            return query;
        }
        public async Task<AppMenu> GetMenuByIdAsync(int id)
        {
            return await _context.AppMenu
                        //.Include(m => m.MenuItems)
                        .FindAsync(id);
        }

        public async Task<AppMenu> InsertMenuAsync(AppMenu menu)
        {
            await _context.AddAsync(menu);
            await _context.SaveChangesAsync();
            return menu;
        }

        public async Task<AppMenu> UpdateMenuAsync(AppMenu menu)
        {
            var consulta = await _context.AppMenu.FindAsync(menu.Id);
            if (consulta == null)
            {
                return null;
            }
            _context.Entry(consulta).CurrentValues.SetValues(menu);
            await _context.SaveChangesAsync();

            return consulta;
        }
    }

}