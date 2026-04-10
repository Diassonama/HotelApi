using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Tenant.Entities;

namespace Hotel.Application.Interfaces
{
    public interface ITenantRepository
    {
        Task<Tenant> GetTenantByIdAsync(string tenantId);
        Task AddTenantAsync(Tenant tenant);
    }
}