using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Interfaces;
using Hotel.Infrastruture.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Services
{
    public class ConfigService : IConfigService
    {
         private readonly GhotelDbContext _context;

        public ConfigService(GhotelDbContext context)
        {
            _context = context;
        }

        public string Getvalue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The configuration key cannot be null or empty.", nameof(value));
            }
            // ✅ CORREÇÃO: Usar método assíncrono em vez de síncrono
            return GetConfigValueAsync(value).GetAwaiter().GetResult();
        }

        private async Task<string> GetConfigValueAsync(string key)
        {
            // ✅ CORREÇÃO: Usar FirstOrDefaultAsync em vez de FirstOrDefault
            return await _context.AppConfig
                .Where(entry => entry.Key == key)
                .Select(entry => entry.Value)
                .FirstOrDefaultAsync();
        }

        // ✅ NOVO: Método assíncrono para uso futuro
        public async Task<string> GetValueAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The configuration key cannot be null or empty.", nameof(key));
            }
            return await GetConfigValueAsync(key);
        }
    }
}