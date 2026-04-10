using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IContaPagarRepository : IRepositoryBase<ContaPagar>
    {
        Task<ContaPagar> GetByIdAsync(int id);
        Task<List<ContaPagar>> GetByEmpresaAsync(int empresaId);
        Task<List<ContaPagar>> GetPendentesAsync();
        Task<List<ContaPagar>> GetRelatorioAsync(int? empresaId, DateTime? dataInicio, DateTime? dataFim);
    }
}