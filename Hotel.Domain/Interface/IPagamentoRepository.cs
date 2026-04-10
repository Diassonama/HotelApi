using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IPagamentoRepository : IRepositoryBase<Pagamento>
    {
        Task<IEnumerable<Pagamento>> GetPagamentosAsync();
        Task<List<Pagamento>> GetAllByCheckinIdAsync(int checkinId);
        Task<Pagamento> GetByIdAsync(int Id);
        Task<Pagamento> GetByIdHospedagemAsync(int Id);
        Task<Pagamento> GetByCheckinIdAsync(int Id);
        Task<Pagamento> GetByCheckinIdTop1Async(int Id);
        Task<IPaginatedList<Pagamento>> GetFilteredApartamentoquery(PaginationFilter paginationFilter);
        IQueryable GetFilteredAsync(PaginationFilter paginationFilter);
        Task<Pagamento> AddAsync(Pagamento pagamentoRequest);
        Task<PagamentoValorTotalResponse> GetValorTotalByCheckinIdAsync(int checkinId);
    }
}