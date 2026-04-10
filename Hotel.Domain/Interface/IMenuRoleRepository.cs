using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;

namespace Hotel.Domain.Interface
{
    public interface IMenuRoleRepository
    {
        Task<IEnumerable<MenuRole>> GetMenuAsync();
        //   Task<IEnumerable<MenuRole>> GetMenuItemByRoleAsync(string perfil);
        Task<IEnumerable<MenuRole>> GetMenuItemByRoleAsync(string perfil);

        Task<MenuRole> GetMenuByIdAsync(int id);
        Task<MenuRole[]> InsertMenuAsync(MenuRole[] menu);
        // Task<MenuRole[]> InsertMenuAsync(MenuRole[] menu);
        Task<MenuRole> DeleteMenuAsync(string id);

    }
}