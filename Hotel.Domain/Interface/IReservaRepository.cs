using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IReservaRepository : IRepositoryBase<Reserva>
    {
        Task<Reserva> InserirReservaComApartamentosAsync(Reserva reserva, List<ApartamentosReservado> apartamentosReservados);
        Task<bool> InserirApartamentosReservadosAsync(int reservaId, List<ApartamentosReservado> apartamentosReservados);
        Task<Reserva> ObterReservaComApartamentosAsync(int reservaId);
        Task<bool> CancelarApartamentosReservadosAsync(List<int> apartamentosReservadosIds);
        Task<IEnumerable<Reserva>> ObterTodasReservaComApartamentosAsync();
        Task<IPaginatedList<Reserva>> GetFilteredReservaquery(PaginationFilter paginationFilter);
        Task<IPaginatedList<ApartamentosReservado>> GetFilteredApartamentosReservadosquery(PaginationFilter paginationFilter);
        IQueryable GetFilteredAsync(PaginationFilter paginationFilter);
        Task<(bool IsDisponivel, string MensagemErro)> VerificarDisponibilidadeAsync(int apartamentoId, DateTime dataEntrada, DateTime dataSaida);
        Task<(bool IsDisponivel, string MensagemErro)> VerificarDisponibilidadeAsync(int apartamentoId, DateTime dataEntrada, DateTime dataSaida, int? reservaIdExcluir);
        Task<bool> VerificarHospedagensAtivasAsync(int reservaId);
 
    }
}