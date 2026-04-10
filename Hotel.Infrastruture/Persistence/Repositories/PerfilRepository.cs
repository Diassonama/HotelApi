using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class PerfilRepository : RepositoryBase<Perfil>, IPerfilRepository
    {
        public PerfilRepository(GhotelDbContext dbContext) : base(dbContext)
        {
        }
    }
}