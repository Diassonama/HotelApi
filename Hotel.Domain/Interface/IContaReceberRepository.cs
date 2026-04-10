using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IContaReceberRepository : IRepositoryBase<ContaReceber>
    {
        Task<ContaReceber> GetByIdAsync(int id);
        Task<List<ContaReceber>> GetByEmpresaAsync(int empresaId);
        Task<ContaReceber> GetByCheckinIdAsync(int checkinId);
        Task<List<ContaReceber>> GetRelatorioAsync(int? empresaId, DateTime? dataInicio, DateTime? dataFim);
    }
}