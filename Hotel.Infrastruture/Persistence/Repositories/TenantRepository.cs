using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Interfaces;
using Hotel.Domain.Tenant.Entities;
using Hotel.Infrastruture.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly TenantContext _context;

        public TenantRepository(TenantContext context)
        {
            _context = context;
        }

        public async Task<Tenant> GetTenantByIdAsync(string tenantId)
        {
            var tenantEntity = await _context.Set<Tenant>()
                .FirstOrDefaultAsync(t => t.Id == tenantId);

            if (tenantEntity == null) return null;


            return tenantEntity;
/* 
            return new Tenant
            {
                Id = tenantEntity.Id,
                Name = tenantEntity.Name,
                ConnectionString = tenantEntity.ConnectionString,
                DatabaseName = tenantEntity.DatabaseName,
                DatabaseServerName = tenantEntity.DatabaseServerName,
                UserID = tenantEntity.UserID,
                Password = tenantEntity.Password
                
             //   Metadata = JsonSerializer.Deserialize<TenantMetadata>(tenantEntity.MetadataJson)
            }; */
        }

        public async Task AddTenantAsync(Tenant tenant)
        {
            _context.Set<Tenant>().Add(tenant);
            await _context.SaveChangesAsync();
        }
    }
}