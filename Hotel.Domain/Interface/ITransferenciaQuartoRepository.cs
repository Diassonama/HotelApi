using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface ITransferenciaQuartoRepository : IRepositoryBase<TransferenciaQuarto>
    {
        Task<IEnumerable<TransferenciaQuarto>> GetByCheckinIdAsync(int checkinId);
        Task<IEnumerable<TransferenciaQuarto>> GetByHospedagemOrigemIdAsync(int hospedagemId);
        Task<IEnumerable<TransferenciaQuarto>> GetByHospedagemDestinoIdAsync(int hospedagemId);
        Task<IEnumerable<TransferenciaQuarto>> GetByMotivoIdAsync(int motivoId);
        Task<IEnumerable<TransferenciaQuarto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    }
}