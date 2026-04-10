using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.Interfaces
{
    public interface ITenantCacheService
    {
        T Get<T>(string key, string tenantId);
        void Set<T>(string key, T value, string tenantId, TimeSpan expiration);
    }
}