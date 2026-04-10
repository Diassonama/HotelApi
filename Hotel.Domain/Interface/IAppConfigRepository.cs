using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Interface.Shared;
using Hotel.Domain.Entities;

namespace Hotel.Domain.Interface
{
    public interface IAppConfigRepository: IRepositoryBase<AppConfig> 
    {
        Task<AppConfig> GetByKeyAsync(string key);
    }
}