using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class PlanoDeContaRepository : RepositoryBase<PlanoDeConta>, IPlanoDeContaRepository
    {
         private readonly GhotelDbContext _context;

        public PlanoDeContaRepository(GhotelDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
         public async Task<PlanoDeConta> GetByNameAsync(string name)
        {
            return await _context.PlanoDeContas.FirstOrDefaultAsync(p => p.Descricao == name);

        }
    }
}