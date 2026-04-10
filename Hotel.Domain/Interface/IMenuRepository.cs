using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IMenuRepository: IRepositoryBase<AppMenu>
    {
         Task<IEnumerable<AppMenu>> GetMenuAsync();

        Task<IEnumerable<MenuRole>> GetMenuByRoleAsync(string perfil);
        Task<bool> HaveAcess(string AppMenu, string Perfil);
        Task<AppMenu> GetMenuByIdAsync(int id);
        Task<AppMenu> InsertMenuAsync(AppMenu AppMenu);
        Task<AppMenu> UpdateMenuAsync(AppMenu AppMenu);
        Task<AppMenu> DeleteMenuAsync(int id);
        Task<IEnumerable<MenuAcesso>> GetMenuAcessobyRoleAsync(string id);
        IQueryable<AppMenu> GetFilteredAsync(PaginationFilter paginationFilter);
    }

}