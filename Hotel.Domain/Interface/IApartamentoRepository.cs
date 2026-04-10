using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IApartamentoRepository : IRepositoryBase<Apartamentos>
    {
        Task<IPaginatedList<Apartamentos>> ConsultaTodosWithPagging(int page, int pagesize, string searchTerm);
        Task<IEnumerable<Apartamentos>> GetApartamentoAsync();
        Task<Apartamentos> GetByIdAsync(int Id);
        IQueryable GetFilteredAsync(PaginationFilter paginationFilter);
        void ocuparApartamento(int IdApartamento, int CheckinsId);
        void desocuparApartamento(int IdApartamento);
        Task<IPaginatedList<Apartamentos>> GetFilteredApartamentoquery(PaginationFilter paginationFilter);

        Task<(List<QuartoStatusDto> QuantidadesPorStatus, int Total)> ObterQuantidadeQuartosPorStatusAsync();
        Task<List<Apartamentos>> GetQuartosAtrazadosAsync();
        Task AtualizarSituacaoApartamentosAsync();
        Task<List<Apartamentos>> GetBySituacaoAsync(Situacao situacao);
        Task<List<Apartamentos>> GetApartamentosOcupadosAsync();
        Task<IEnumerable<Apartamentos>> GetApartamentoOcupadosAsync();
       // Task<IEnumerable<Apartamentos>> GetApartamentosOcupadosSemIncludeAsync();
        Task<List<Apartamentos>> GetApartamentosOcupadosSemIncludeAsync();
        Task<List<ApartamentoComCheckinAtivoDto>> GetApartamentosComCheckinAtivoAsync();
        Task<List<Apartamentos>> GetApartamentosOcupados2Async();
        
       
    }
}