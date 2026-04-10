using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IHospedagemRepository: IRepositoryBase<Hospedagem>
    {
        Task<int> AddAsync(Hospedagem checkins);
        Task<IEnumerable<Hospedagem>> GetHospedagemAsync();
        Task<Hospedagem> GetByIdAsync(int Id);
        Task<Hospedagem> GetByIdAsyncAsNotrack(int Id)
;        Task<Hospedagem> GetByCheckinIdAsync(int Id);
        IQueryable GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter);
    }
}