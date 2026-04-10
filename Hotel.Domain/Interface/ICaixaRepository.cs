using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface ICaixaRepository : IRepositoryBase<Caixa>
    {

        Task<Caixa> GetByIdAsync(int id);
        Task<IEnumerable<Caixa>> GetAllAsync(   string usuarioId, string perfil);
        Task<int> getCaixa();
        Task<IPaginatedList<Caixa>> GetFilteredquery(PaginationFilter paginationFilter, string usuarioId, string perfil);
        IQueryable<Caixa> GetFilteredAsync(PaginationFilter paginationFilter, string usuarioId, string perfil);
        Task<Caixa> GetByDateAsync(DateTime data, string usuarioId, string perfil);
        Task<object> MovimentoDoCaixa( string usuarioId, string perfil);
         Task<object> MovimentoDoCaixaPorUsuario(string usuarioId, string perfil);
    }
}