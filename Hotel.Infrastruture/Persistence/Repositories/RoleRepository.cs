using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Enums;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class RoleRepository : RepositoryBase<IdentityRole>, IRoleRepository
    {
        private readonly GhotelDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleRepository(GhotelDbContext context, RoleManager<IdentityRole> roleManager) : base(context)
        {
            _context = context;
            _roleManager = roleManager;
        }
        public IQueryable<Roles> GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            var query = _context.Roles;

           /*  if (paginationFilter?.FieldFilter is not null && !string.IsNullOrWhiteSpace(paginationFilter.FieldFilter))
            {
                var fieldFilter = paginationFilter.FieldFilter.ToLower();
                query = query.Where(r => r.Name.ToString().Contains(fieldFilter));
            } */

            return (IQueryable<Roles>)query;
        }

        public async Task<IEnumerable<IdentityRole>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<IdentityRole> GetByIdAsync(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }

        public async Task<IdentityRole> GetByNameAsync(string name)
        {
            return await _roleManager.FindByNameAsync(name);
        }

        public async Task AddAsync(IdentityRole role)
        {
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task UpdateAsync(IdentityRole role)
        {
            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task DeleteAsync(string id)
        {
            var role = await GetByIdAsync(id);
            if (role == null) throw new Exception("Role not found");

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

    }
}