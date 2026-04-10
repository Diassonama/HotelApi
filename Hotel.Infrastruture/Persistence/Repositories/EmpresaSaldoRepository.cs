using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class EmpresaSaldoRepository : RepositoryBase<EmpresaSaldo>, IEmpresaSaldoRepository
    {
        private readonly GhotelDbContext _context;

        public EmpresaSaldoRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém o saldo de uma empresa pelo ID
        /// </summary>
        public async Task<EmpresaSaldo> GetByEmpresaIdAsync(int empresaId)
        {
            return await _context.EmpresaSaldos
                .Include(s => s.EmpresaSaldoMovimentos)
                .Include(s => s.Empresa)
                .FirstOrDefaultAsync(s => s.EmpresaId == empresaId);
        }

        /// <summary>
        /// Processa movimentação de saldo (crédito ou débito)
        /// Implementação baseada no stored procedure [dbo].[Saldo]
        /// </summary>
        public async Task ProcessarMovimentacaoSaldoAsync(
            int empresaId, 
            decimal valor, 
            TipoLancamento tipoLancamento, 
            string documento, 
            string utilizadorId,
            string observacao = null)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor deve ser positivo.");
            if (string.IsNullOrWhiteSpace(utilizadorId))
                throw new ArgumentException("Utilizador é obrigatório.");

            var empresaSaldo = await GetByEmpresaIdAsync(empresaId);

            // Se não existe saldo, cria um novo registro
            if (empresaSaldo == null)
            {
              /*   empresaSaldo = new EmpresaSaldo(empresaId, tipoLancamento == TipoLancamento.E ? valor : 0);
                _context.EmpresaSaldos.Add(empresaSaldo);
                await _context.SaveChangesAsync(); */
                    empresaSaldo = new EmpresaSaldo(empresaId, 0); // ← sempre 0, movimento aplicado depois
    _context.EmpresaSaldos.Add(empresaSaldo);
    await _context.SaveChangesAsync();
            }

            // Processa a movimentação
            if (tipoLancamento == TipoLancamento.S)
            {
                // Débito: diminui o saldo (uso do saldo pré-pago)
                empresaSaldo.DebitarSaldo(valor);
            }
            else if (tipoLancamento == TipoLancamento.E)
            {
                // Crédito: aumenta o saldo (adiantamento/depósito)
                empresaSaldo.AdicionarSaldo(valor);
            }
            else
            {
                throw new ArgumentException("Tipo de lançamento inválido. Use Crédito ou Débito.");
            }

            // Registra o item de movimentação (histórico)
            var movimento = new EmpresaSaldoMovimento(
                empresaSaldo.Id, 
                valor, 
                tipoLancamento, 
                documento, 
                utilizadorId, 
                observacao);
            
            _context.EmpresaSaldoMovimentos.Add(movimento);

            // Atualiza e salva
            //await Update(empresaSaldo);
                // ✅ Agora salva tudo de uma vez
            //    _context.EmpresaSaldos.Update(empresaSaldo);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtém o histórico de movimentações de uma empresa
        /// </summary>
        public async Task<List<EmpresaSaldoMovimento>> GetMovimentacoesAsync(int empresaId)
        {
            return await _context.EmpresaSaldoMovimentos
            .AsNoTracking()
            .Include(e=> e.EmpresaSaldo).ThenInclude(es=> es.Empresa)
                .Include(m => m.Utilizador)
                .Where(s => s.EmpresaSaldo.EmpresaId == empresaId)
                .OrderByDescending(s => s.DateCreated)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém o saldo total de todas as empresas
        /// </summary>
        public async Task<List<EmpresaSaldo>> GetTodosSaldosAsync()
        {
            return await _context.EmpresaSaldos

            .Include(e=> e.Empresa)
                .Include(s => s.Empresa)
                .Include(s => s.EmpresaSaldoMovimentos)
                .ToListAsync();
        }

        /// <summary>
        /// Consulta movimentações entre datas
        /// </summary>
        public async Task<List<EmpresaSaldoMovimento>> GetMovimentacoesEntreDatasAsync(
            int empresaId, 
            DateTime dataInicio, 
            DateTime dataFim)
        {
            return await _context.EmpresaSaldoMovimentos
            .AsNoTracking()
                .Include(m => m.Utilizador)
                .Include(e=> e.EmpresaSaldo).ThenInclude(es=> es.Empresa)
                .Where(s => s.EmpresaSaldo.EmpresaId == empresaId 
                    && s.DateCreated >= dataInicio 
                    && s.DateCreated <= dataFim)
                .OrderByDescending(s => s.DateCreated)
                .ToListAsync();
        }

        public async Task<List<EmpresaSaldoMovimento>> GetMovimentacoesRelatorioAsync(
            int? empresaId,
            DateTime? dataInicio,
            DateTime? dataFim)
        {
            var query = _context.EmpresaSaldoMovimentos
                .AsNoTracking()
                .Include(m => m.Utilizador)
                .Include(m => m.EmpresaSaldo)
                    .ThenInclude(es => es.Empresa)
                .AsQueryable();

            if (empresaId.HasValue)
                query = query.Where(m => m.EmpresaSaldo.EmpresaId == empresaId.Value);

            if (dataInicio.HasValue)
                query = query.Where(m => m.DateCreated >= dataInicio.Value.Date);

            if (dataFim.HasValue)
            {
                var fim = dataFim.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(m => m.DateCreated <= fim);
            }

            return await query
                .OrderByDescending(m => m.DateCreated)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém o saldo atual de uma empresa
        /// </summary>
        public async Task<decimal> GetSaldoAtualAsync(int empresaId)
        {
            var empresaSaldo = await GetByEmpresaIdAsync(empresaId);
            return empresaSaldo?.Saldo ?? 0;
        }

        /// <summary>
        /// Verifica se a empresa tem saldo suficiente para uma operação
        /// </summary>
        public async Task<bool> VerificarSaldoSuficienteAsync(int empresaId, decimal valor)
        {
            var saldoAtual = await GetSaldoAtualAsync(empresaId);
            return saldoAtual >= valor;
        }
    }
}