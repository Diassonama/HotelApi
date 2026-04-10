using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IApartamentoReservadoRepository: IRepositoryBase<ApartamentosReservado>
    {
        Task<bool> VerificarDisponibilidadeAsync(int roomId, DateTime startDate, DateTime endDate);
        Task<bool> VerificarDisponibilidadeAsync(int roomId, DateTime startDate, DateTime endDate, int? excludeReservationId);
        Task<IEnumerable<ApartamentosReservado>> ObterReservasComDadosCompletosAsync();
        Task<IEnumerable<ApartamentosReservado>> ObterReservasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<ApartamentosReservado>> ObterReservasPorApartamentoAsync(int apartamentoId);
        Task<Reserva> ObterReservaComDetalhesAsync(int reservaId);
    }
}