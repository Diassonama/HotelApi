using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IPlanoDeContaRepository : IRepositoryBase<PlanoDeConta>
    {
        Task<PlanoDeConta> GetByNameAsync(string name);
        
    }
}