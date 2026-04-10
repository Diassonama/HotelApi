using System;
using System.Collections.Generic;
using Hotel.Application.DTOs;

namespace Hotel.Application.Interfaces
{
    public interface IRelatorioContasService
    {
        /// <summary>
        /// Gera relatório de Contas a Receber
        /// </summary>
        byte[] GerarRelatorioContasReceber(
            List<ContaReceberDto> contas,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string filtroEmpresa = null);

        /// <summary>
        /// Gera relatório de Contas a Pagar
        /// </summary>
        byte[] GerarRelatorioContasPagar(
            List<ContaPagarDto> contas,
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string filtroFornecedor = null);

        /// <summary>
        /// Gera relatório consolidado (Receber + Pagar)
        /// </summary>
        byte[] GerarRelatorioFluxoCaixa(
            List<ContaReceberDto> contasReceber,
            List<ContaPagarDto> contasPagar,
            DateTime? dataInicio = null,
            DateTime? dataFim = null);
    }
}