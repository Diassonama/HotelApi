using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IHistoricoRepository: IRepositoryBase<Historico>
    {
        Task<List<Historico>> GetAllByCheckinIdAsync(int checkinId);
        Task<List<Historico>> GetAllByDateAsync(DateTime date);
        Task<List<Historico>> GetHistoricoFechamentoCaixaAsync(DateTime date, int? caixaId);
    }
}