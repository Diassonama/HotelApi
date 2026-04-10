using System;
using System.Collections.Generic;
using Hotel.Application.DTOs;

namespace Hotel.Application.Interfaces
{
    public interface IRelatorioSaldoService
    {
        /// <summary>
        /// Gera relatório completo de movimentações (créditos e débitos)
        /// </summary>
        byte[] GerarRelatorioMovimentacoes(
            string nomeEmpresa,
            decimal saldoAtual,
            List<EmpresaSaldoMovimentoDto> movimentacoes,
            DateTime? dataInicio = null,
            DateTime? dataFim = null);

        /// <summary>
        /// Gera relatório apenas de créditos
        /// </summary>
        byte[] GerarRelatorioCreditos(
            string nomeEmpresa,
            List<EmpresaSaldoMovimentoDto> creditos,
            DateTime? dataInicio = null,
            DateTime? dataFim = null);

        /// <summary>
        /// Gera relatório apenas de débitos
        /// </summary>
        byte[] GerarRelatorioDebitos(
            string nomeEmpresa,
            List<EmpresaSaldoMovimentoDto> debitos,
            DateTime? dataInicio = null,
            DateTime? dataFim = null);

        /// <summary>
        /// Gera relatório de adiantamentos (saldos) e seu histórico de movimentações
        /// </summary>
        byte[] GerarRelatorioAdiantamentosHistorico(
            string nomeEmpresaFiltro,
            List<EmpresaSaldoDto> saldos,
            List<EmpresaSaldoMovimentoDto> movimentacoes,
            DateTime? dataInicio = null,
            DateTime? dataFim = null);
    }
}