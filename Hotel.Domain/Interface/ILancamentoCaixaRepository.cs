using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface ILancamentoCaixaRepository: IRepositoryBase<LancamentoCaixa>
    {
         Task<LancamentoCaixa> GetByPagamentoIdAsync(int Id);
         Task<List<LancamentoCaixa>> GetAllByDateAsync(DateTime date);
         Task<List<LancamentoCaixa>> GetMovimentoCaixaAsync(DateTime? dataInicio, DateTime? dataFim, string perfil, string usuario);
            Task<List<LancamentoCaixa>> GetPagamentosFechamentoCaixaAsync(DateTime date, int? caixaId, string perfil, string usuario);
    }
}