using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;


namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class AppConfigRepository: RepositoryBase<AppConfig>, IAppConfigRepository
    {
         private readonly GhotelDbContext _context;
        public AppConfigRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }

         public async Task<AppConfig> GetByKeyAsync(string key)
        {
            return await _context.AppConfig.FirstOrDefaultAsync(p => p.Key == key);

        }
        
    }
}