using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Dtos;
using Hotel.Domain.DTOs;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IFacturaEmpresaRepository : IRepositoryBase<FacturaEmpresa>
    {
        Task<IEnumerable<FacturaEmpresa>> GetFacturaEmpresaAsync();
        Task<FacturaEmpresa> GetByIdAsync(int Id);
        Task<FacturaEmpresa> GetByCheckinsIdAsync(int Id);
        Task<FacturaEmpresa> GetByIdEmpresaAsync(int Id);
        Task<IPaginatedList<FacturaEmpresa>> GetFilteredQuery(PaginationFilter paginationFilter);

        IQueryable<FacturaEmpresa> GetFilteredAsync(PaginationFilter paginationFilter);
        Task<IEnumerable<object>> EmpresasComDividas();
        IQueryable<FacturaEmpresaDto> GetFilteredEmpresasComDividasAsync(PaginationFilter paginationFilter);
        IQueryable<FacturaEmpresa> GetFilteredDetalhesDeDividasEmpresaAsync(PaginationFilter paginationFilter, int Id);
        IQueryable<FacturaEmpresaDetalhesDto> GetFilteredDetalhesDeDividasEmpresaAsyncV2(Domain.Interface.Shared.PaginationFilter paginationFilter, int Id);

        (IQueryable<FacturaEmpresa> Registros, float ValorTotal) GetFilteredDetalhesDeDividasEmpresaComTotalAsync(Domain.Interface.Shared.PaginationFilter paginationFilter, int Id);

        Task<int> AddEmpresaAsync(FacturaEmpresa checkins);

    }
}