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
    public class TipoHospedagemRepository : RepositoryBase<TipoHospedagem>, ITipoHospedagemRepository
    {
        readonly GhotelDbContext _dbContext;
        public TipoHospedagemRepository(GhotelDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TipoHospedagem> GetbyName(string name)
        {
            return await _dbContext.TipoHospedagens.FirstOrDefaultAsync(n => n.Descricao == name);
        }

       
    }
}