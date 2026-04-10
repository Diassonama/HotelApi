using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface ICheckinRepository: IRepositoryBase<Checkins>
    {
         Task<IEnumerable<Checkins>> GetApartamentoAsync();
         Task<Checkins> GetByIdAsync(int Id);
         Task<IPaginatedList<Checkins>> GetFilteredApartamentoquery(Domain.Interface.Shared.PaginationFilter paginationFilter);
         IQueryable<Checkins> GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter);
         Task<int> AddCheckinsAsync(Checkins checkins);
         void AtualizarPrevisaoParaAtrasados();
         Task RealizarCheckoutAsync(int checkinId, int hospedeId, DateTime dataHoraSaida);
         // Hotel.Domain/Interface/Shared/IRepositoryBase.cs - adicionar
         Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);
         Task<List<Checkins>> GetCheckinsByDateAsync(DateTime date);
         Task<List<Checkins>> GetCheckoutsByDateAsync(DateTime date);
         Task<List<Checkins>> GetCheckinsFechamentoCaixaAsync(DateTime date, int? caixaId);
         Task<List<Checkins>> GetCheckoutsFechamentoCaixaAsync(DateTime date, int? caixaId);
    }
}