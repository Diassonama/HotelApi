using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface ITransferenciaRepository : IRepositoryBase<Transferencia>
    {
        Task<IEnumerable<Transferencia>> GetByCheckinIdAsync(int checkinId);
        Task<IEnumerable<Transferencia>> GetByHospedagemOrigemIdAsync(int hospedagemId);
        Task<IEnumerable<Transferencia>> GetByHospedagemDestinoIdAsync(int hospedagemId);
        Task<IEnumerable<Transferencia>> GetByMotivoIdAsync(int motivoId);
        Task<IEnumerable<Transferencia>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<int> GetCountAsync();
        Task<int> GetCountByCheckinIdAsync(int checkinId);
        Task<int> GetCountByPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    }
}