using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Hotel.Domain.Interface;
using Hotel.Application.Interfaces;
using Hotel.Application.Dtos;
using Hotel.Application.Services;

namespace Hotel.Application.Reports.Commands
{
    public class GerarReciboCheckoutCommand: IRequest<BaseCommandResponse>
    {
        public int CheckinId { get; set; }
    }
        public class GerarReciboCheckoutCommandHandler : IRequestHandler<GerarReciboCheckoutCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReciboService _reciboService;
        private UsuarioLogado logado;
        private readonly ILogger<GerarReciboCheckoutCommandHandler> _logger;

        public GerarReciboCheckoutCommandHandler(
            IUnitOfWork unitOfWork,
            IReciboService reciboService,
            ILogger<GerarReciboCheckoutCommandHandler> logger,
            UsuarioLogado logado)
        {
            _unitOfWork = unitOfWork;
            _reciboService = reciboService;
            _logger = logger;
            this.logado = logado;
        }

        public async Task<BaseCommandResponse> Handle(GerarReciboCheckoutCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("📄 [GERAR-RECIBO-{CorrelationId}] Gerando recibo para CheckinId: {CheckinId}",
                    correlationId, request.CheckinId);

                // ✅ BUSCAR CHECK-IN COM RELACIONAMENTOS
                var checkin = await _unitOfWork.checkins.GetByIdAsync(request.CheckinId);
                if (checkin == null)
                {
                    _logger.LogError("❌ [GERAR-RECIBO-{CorrelationId}] Check-in não encontrado: {CheckinId}",
                        correlationId, request.CheckinId);
                    throw new ArgumentException("Check-in não encontrado.");
                }

                // ✅ BUSCAR HOSPEDAGEM
                var hospedagem = await _unitOfWork.Hospedagem.GetByCheckinIdAsync(request.CheckinId);
                if (hospedagem == null)
                {
                    _logger.LogError("❌ [GERAR-RECIBO-{CorrelationId}] Hospedagem não encontrada para CheckinId: {CheckinId}",
                        correlationId, request.CheckinId);
                    throw new ArgumentException("Hospedagem não encontrada.");
                }

                var parametros = await _unitOfWork.Parametros.Get(1);
                if (parametros == null)
                {
                    _logger.LogError("❌ [GERAR-RECIBO-{CorrelationId}] Parâmetros do sistema não encontrados",
                        correlationId);
                    throw new ArgumentException("Parâmetros do sistema não encontrados.");
                }


                // ✅ BUSCAR HÓSPEDE
                var hospede = await _unitOfWork.hospedes.GetByCheckinIdAsync(request.CheckinId);
                if (hospede == null)
                {
                    _logger.LogError("❌ [GERAR-RECIBO-{CorrelationId}] Hóspede não encontrado para HospedeId: {HospedeId}",
                        correlationId, hospedagem.Id);
                    throw new ArgumentException("Hóspede não encontrado.");
                }

                // ✅ BUSCAR APARTAMENTO
                var apartamento = await _unitOfWork.Apartamento.GetByIdAsync(hospedagem.ApartamentosId);
                if (apartamento == null)
                {
                    _logger.LogError("❌ [GERAR-RECIBO-{CorrelationId}] Apartamento não encontrado para ApartamentoId: {ApartamentoId}",
                        correlationId, hospedagem.ApartamentosId);
                    throw new ArgumentException("Apartamento não encontrado.");
                }
                var pagamento = await _unitOfWork.pagamentos.GetByCheckinIdTop1Async(request.CheckinId);
                if(pagamento == null)
                {
                    _logger.LogError("❌ [GERAR-RECIBO-{CorrelationId}] Pagamento não encontrado para CheckinId: {CheckinId}",
                        correlationId, request.CheckinId);
                    throw new ArgumentException("Pagamento não encontrado.");
                }

                var movimentoCaixa = await _unitOfWork.lancamentoCaixas.GetByPagamentoIdAsync(pagamento.Id);
                if(movimentoCaixa == null)
                {
                    _logger.LogError("❌ [GERAR-RECIBO-{CorrelationId}] Pagamento não encontrado para PagamentoId: {PagamentoId}",
                        correlationId, pagamento.Id);
                    throw new ArgumentException("Movimento de caixa não encontrado."); 
                }
             /*    var utilizador = await _unitOfWork.Utilizadores.GetByIdAsync(logado.IdUtilizador);
                if(utilizador == null)
                {
                    _logger.LogError("❌ [GERAR-RECIBO-{CorrelationId}] Usuário não encontrado para UtilizadorId: {UtilizadorId}",
                        correlationId, logado.IdUtilizador);
                    throw new ArgumentException("Usuário não encontrado.");
                } */

                // ✅ BUSCAR DADOS DA EMPRESA/HOTEL
                var empresa = await _unitOfWork.Empresa.Get(hospedagem.EmpresasId);
                if (empresa == null)
                {
                    _logger.LogError("❌ [GERAR-RECIBO-{CorrelationId}] Empresa não encontrada para EmpresaId: {EmpresaId}",
                        correlationId, hospedagem.EmpresasId);
                    throw new ArgumentException("Empresa não encontrada.");
                }

                _logger.LogInformation("✅ [GERAR-RECIBO-{CorrelationId}] Todos os dados coletados com sucesso",
                    correlationId);

                // ✅ CONSTRUIR DTO DO RECIBO
                var reciboDto = new ReciboCheckoutDto
                {
                    
                    NomeHotel = parametros.NomeEmpresa,
                    Endereco = parametros.Endereco,
                    Cidade = parametros.Cidade,
                    NumContribuinte = parametros.NumContribuinte,
                    CheckinNumero = checkin.Id,
                    NomeHospede = hospede.Clientes.Nome,
                    ApartamentoCodigo = apartamento.Codigo,
                    TipoApartamento = apartamento.TipoApartamentos.Descricao,
                    DataEntrada = hospedagem.DataAbertura,
                    DataSaida = hospedagem.PrevisaoFechamento ,
                    NumDias = hospedagem.QuantidadeDeDiarias, 
                    DataImpressao = DateTime.Now,
                    ValorDiaria = hospedagem.ValorDiaria,
                    ValorDiarias = checkin.ValorTotalDiaria,
                    Consumo = checkin.ValorTotalConsumo,
                    Desconto = checkin.ValorDesconto,
                    Total = checkin.ValorTotalFinal,
                    Pago = await ObterValorPago(request.CheckinId),
                    APagar = checkin.ValorTotalFinal - await ObterValorPago(request.CheckinId),
                    Operador = await _unitOfWork.Utilizadores.GetNomeCompletoByIdAsync( movimentoCaixa.UtilizadoresId),// movimentoCaixa.UtilizadoresId ?? "Sistema", //    checkin.IdUtilizadorCheckOut ?? "Sistema",
                    FormaPagamento = movimentoCaixa.TipoPagamentos.Descricao,
                    DecretoFiscal = "DECRETO Nº 18/92 D.R.I",
                    TipoHospede = hospede.Estado.ToString()
                };

                _logger.LogInformation("📝 [GERAR-RECIBO-{CorrelationId}] DTO do recibo construído - Total: {Total}, APagar: {APagar}",
                    correlationId, reciboDto.Total, reciboDto.APagar);

                // ✅ GERAR PDF
                var pdfBytes = _reciboService.GerarReciboCheckout(reciboDto);

                _logger.LogInformation("✅ [GERAR-RECIBO-{CorrelationId}] PDF gerado com sucesso - Tamanho: {TamanhoPDF} bytes",
                    correlationId, pdfBytes.Length);

                response.Success = true;
                response.Message = "Recibo gerado com sucesso";
                response.Data = new
                {
                    pdf = Convert.ToBase64String(pdfBytes),
                    nomeArquivo = $"Recibo_Checkin_{checkin.Id}.pdf",
                    checkinId = checkin.Id,
                    hospedagem = hospedagem.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GERAR-RECIBO-{CorrelationId}] Erro ao gerar recibo para CheckinId: {CheckinId}: {Message}",
                    correlationId, request.CheckinId, ex.Message);

                response.Success = false;
                response.Message = $"Erro ao gerar recibo: {ex.Message}";
            }

            return response;
        }

        private async Task<float> ObterValorPago(int checkinId)
        {
            var resultado = await _unitOfWork.pagamentos.GetValorTotalByCheckinIdAsync(checkinId);
            return resultado?.ValorTotalPago ?? 0;
        }
    }
}