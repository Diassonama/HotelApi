using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Dtos;
using Hotel.Application.DTOs;

namespace Hotel.Application.Interfaces
{
    public interface IReciboService
    {
        byte[] GerarReciboCheckout(ReciboCheckoutDto recibo);
        byte[] GerarNotaHospedagem(NotaHospedagemDto nota);
        byte[] GerarMovimentoDiario(MovimentoDiarioDto movimento);
        byte[] GerarMovimentoCaixa(MovimentoCaixaDto movimento);
        byte[] GerarHistoricoOcupacao(IEnumerable<HistoricoOcupacaoDto> linhas, string titulo, int totalQuartos, int quartosOcupados);
        byte[] GerarRelatorioGovernancaArrumacao(IEnumerable<GovernancaArrumacaoDto> linhas, string titulo);
        byte[] GerarRelatorioVistoriaQuartos(IEnumerable<VistoriaQuartoDto> linhas, string titulo, string funcionario, DateTime dataReferencia);
        void SalvarReciboCheckout(ReciboCheckoutDto recibo, string caminhoSaida);
        byte[] GerarReciboPedido(ReciboPedidoDto recibo);
    }
}