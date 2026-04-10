using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums;

namespace Hotel.Domain.Interface
{
    public interface IEmpresaSaldoRepository
    {
        
        Task<EmpresaSaldo> GetByEmpresaIdAsync(int empresaId);
        Task ProcessarMovimentacaoSaldoAsync(int empresaId, decimal valor, TipoLancamento tipoLancamento, string documento, string utilizadorId, string observacao = null);
        Task<List<EmpresaSaldoMovimento>> GetMovimentacoesAsync(int empresaId);
        Task<List<EmpresaSaldo>> GetTodosSaldosAsync();
        Task<List<EmpresaSaldoMovimento>> GetMovimentacoesEntreDatasAsync(int empresaId, DateTime dataInicio, DateTime dataFim);
        Task<List<EmpresaSaldoMovimento>> GetMovimentacoesRelatorioAsync(int? empresaId, DateTime? dataInicio, DateTime? dataFim);
        Task<decimal> GetSaldoAtualAsync(int empresaId);
        Task<bool> VerificarSaldoSuficienteAsync(int empresaId, decimal valor);
    }
}