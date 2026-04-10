using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IHospedeRepository: IRepositoryBase<Hospede>
    {
        Task<Hospede> GetByIdAsync(int id);
        Task<Hospede> GetByCheckinIdAsync(int id);
    }
}