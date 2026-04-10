using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Interfaces;
using Hotel.Domain.Tenant.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace Hotel.Infrastruture.Services
{
    public class TenantCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ITenantService _tenantService;

        public TenantCacheService(IMemoryCache cache, ITenantService tenantService)
        {
            _cache = cache;
            _tenantService = tenantService;
        }

        public T GetOrSet<T>(string key, Func<T> getData, TimeSpan expiration)
        {
            var tenantKey = $"{_tenantService.GetCurrentTenant().Id}_{key}";
            if (!_cache.TryGetValue(tenantKey, out T value))
            {
                value = getData();
                _cache.Set(tenantKey, value, expiration);
            }

            return value;
        }
        /*  public async Task<Tenant> GetTenantAsync(string tenantName)
     {
         if (!_cache.TryGetValue(tenantName, out Tenant tenant))
         {
             tenant = await _tenantService.GetTenantAsync(tenantName);
             _cache.Set(tenantName, tenant, TimeSpan.FromHours(1));
         }

         return tenant; */
    }
}
