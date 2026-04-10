using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Interface.Shared;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Domain.Interface
{
    public interface IRoleRepository: IRepositoryBase<IdentityRole>
    {
        Task<IdentityRole> GetByIdAsync(string id);
        Task<IdentityRole> GetByNameAsync(string name);
        Task<IEnumerable<IdentityRole>> GetAllAsync();
        Task AddAsync(IdentityRole role);
        Task UpdateAsync(IdentityRole role);
        Task DeleteAsync(string id);
    }
}