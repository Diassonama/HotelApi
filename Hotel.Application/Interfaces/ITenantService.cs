using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Tenant.Entities;
using Microsoft.Extensions.Configuration;

namespace Hotel.Application.Interfaces
{
    public interface ITenantService
    {
        Task<Tenant> ResolveTenantAsync(string tenantId);
        void SetCurrentTenant(Tenant tenant);
        Tenant GetCurrentTenant();
        Task CreateTenantAsync(Tenant tenant);
        Task ExecuteMigrationsAsync(string connectionString, IConfiguration configuration, ITenantService tenantService);
        Task<Tenant> UpdateAsync(Tenant tenant);
        Task DeleteAsync(string id);
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant> GetByIdAsync(string id);
    }
}