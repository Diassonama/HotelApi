using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hotel.Application.Dtos;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Reports.Commands
{
    public class GerarNotaHospedagemCommand : IRequest<BaseCommandResponse>
    {
        public int CheckinId { get; set; }
    }

    public class GerarNotaHospedagemCommandHandler : IRequestHandler<GerarNotaHospedagemCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReciboService _reciboService;
        private readonly ILogger<GerarNotaHospedagemCommandHandler> _logger;
        private readonly UsuarioLogado _logado;

        public GerarNotaHospedagemCommandHandler(
            IUnitOfWork unitOfWork,
            IReciboService reciboService,
            ILogger<GerarNotaHospedagemCommandHandler> logger,
            UsuarioLogado logado)
        {
            _unitOfWork = unitOfWork;
            _reciboService = reciboService;
            _logger = logger;
            _logado = logado;
        }

        public async Task<BaseCommandResponse> Handle(GerarNotaHospedagemCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("📄 [GERAR-NOTA-{CorrelationId}] Gerando nota de hospedagem para CheckinId: {CheckinId}",
                    correlationId, request.CheckinId);

                var checkin = await _unitOfWork.checkins.GetByIdAsync(request.CheckinId);
                if (checkin == null)
                    throw new ArgumentException("Check-in não encontrado.");

                var hospedagem = await _unitOfWork.Hospedagem.GetByCheckinIdAsync(request.CheckinId);
                if (hospedagem == null)
                    throw new ArgumentException("Hospedagem não encontrada.");

                var parametros = await _unitOfWork.Parametros.Get(1);
                if (parametros == null)
                    throw new ArgumentException("Parâmetros do sistema não encontrados.");

                var hospede = await _unitOfWork.hospedes.GetByCheckinIdAsync(request.CheckinId);
                if (hospede == null)
                    throw new ArgumentException("Hóspede não encontrado.");

                var apartamento = await _unitOfWork.Apartamento.GetByIdAsync(hospedagem.ApartamentosId);
                if (apartamento == null)
                    throw new ArgumentException("Apartamento não encontrado.");

                var empresa = await _unitOfWork.Empresa.Get(hospedagem.EmpresasId);
                var pagamentos = await _unitOfWork.pagamentos.GetAllByCheckinIdAsync(request.CheckinId);
                var historicos = await _unitOfWork.historico.GetAllByCheckinIdAsync(request.CheckinId);
                var valorPago = await ObterValorPago(request.CheckinId);

                var nomeHospede = hospede.Clientes?.Nome ?? "N/D";
                var nomeEmpresa = hospede.Estado == Hospede.EstadoHospede.ContaPropria
                    ? "CONTA PROPRIA"
                    : empresa?.RazaoSocial ?? "N/D";

                var notaDto = new NotaHospedagemDto
                {
                    NomeHotel = parametros.NomeEmpresa,
                    Endereco = parametros.Endereco,
                    Cidade = parametros.Cidade,
                    NumContribuinte = parametros.NumContribuinte,
                    Telefone = parametros.Telefone,
                    LogoCaminho = parametros.LogoCaminho,
                    DataImpressao = ObterDataAngola(),
                    NumeroDocumento = checkin.Id,
                    NomeHospede = nomeHospede,
                    Empresa = nomeEmpresa,
                    UtilizadorCheckin = await ResolverNomeUtilizador(checkin.IdUtilizadorCheckin),
                    UtilizadorCheckout = await ResolverNomeUtilizador(checkin.IdUtilizadorCheckOut),
                    Quarto = apartamento.Codigo,
                    TipoQuarto = apartamento.TipoApartamentos?.Descricao ?? "N/D",
                    DataEntrada = hospedagem.DataAbertura,
                    DataSaida = hospedagem.PrevisaoFechamento,
                    NumDias = hospedagem.QuantidadeDeDiarias,
                    ValorDiaria = hospedagem.ValorDiaria,
                    ValorDiarias = checkin.ValorTotalDiaria,
                    Consumo = checkin.ValorTotalConsumo,
                    Desconto = checkin.ValorDesconto,
                    Total = checkin.ValorTotalFinal,
                    Pago = valorPago,
                    APagar = checkin.ValorTotalFinal - valorPago,
                    Pagamentos = await MapearPagamentosAsync(pagamentos, nomeHospede),
                    Historicos = await MapearHistoricosAsync(historicos)
                };

                var pdfBytes = _reciboService.GerarNotaHospedagem(notaDto);

                response.Success = true;
                response.Message = "Nota de hospedagem gerada com sucesso";
                response.Data = new
                {
                    pdf = Convert.ToBase64String(pdfBytes),
                    nomeArquivo = $"Nota_Hospedagem_Checkin_{checkin.Id}.pdf",
                    checkinId = checkin.Id,
                    hospedagem = hospedagem.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GERAR-NOTA-{CorrelationId}] Erro ao gerar nota para CheckinId: {CheckinId}: {Message}",
                    correlationId, request.CheckinId, ex.Message);

                response.Success = false;
                response.Message = $"Erro ao gerar nota de hospedagem: {ex.Message}";
            }

            return response;
        }

        private async Task<float> ObterValorPago(int checkinId)
        {
            var resultado = await _unitOfWork.pagamentos.GetValorTotalByCheckinIdAsync(checkinId);
            return resultado?.ValorTotalPago ?? 0;
        }

        private async Task<string> ResolverNomeUtilizador(string utilizadorId)
        {
            if (string.IsNullOrWhiteSpace(utilizadorId))
                return await _unitOfWork.Utilizadores.GetNomeCompletoByIdAsync(_logado?.IdUtilizador);

            return await _unitOfWork.Utilizadores.GetNomeCompletoByIdAsync(utilizadorId);
        }

        private async Task<List<NotaHospedagemPagamentoDto>> MapearPagamentosAsync(List<Hotel.Domain.Entities.Pagamento> pagamentos, string nomeHospede)
        {
            var resultado = new List<NotaHospedagemPagamentoDto>();

            foreach (var pagamento in pagamentos)
            {
                var lancamento = pagamento.LancamentoCaixas?
                    .OrderByDescending(l => l.DataHoraLancamento)
                    .FirstOrDefault();

                var operador = MontarNomeCompleto(lancamento?.Utilizadores)
                    ?? MontarNomeCompleto(pagamento.Utilizadores)
                    ?? await ResolverNomeUtilizador(lancamento?.UtilizadoresId ?? pagamento.UtilizadoresId);

                resultado.Add(new NotaHospedagemPagamentoDto
                {
                    Numero = pagamento.Id,
                    Tipo = lancamento?.TipoPagamentos?.Descricao ?? pagamento.Origem ?? "N/D",
                    Data = lancamento?.DataHoraLancamento ?? pagamento.DateCreated,
                    Hospede = pagamento.Hospedes?.Clientes?.Nome ?? nomeHospede,
                    Operador = operador,
                    Valor = pagamento.Valor
                });
            }

            return resultado;
        }

        private async Task<List<NotaHospedagemHistoricoDto>> MapearHistoricosAsync(List<Hotel.Domain.Entities.Historico> historicos)
        {
            var resultado = new List<NotaHospedagemHistoricoDto>();

            foreach (var historico in historicos)
            {
                var operador = MontarNomeCompleto(historico.Utilizadores)
                    ?? await ResolverNomeUtilizador(historico.UtilizadoresId);

                resultado.Add(new NotaHospedagemHistoricoDto
                {
                    Numero = historico.Id,
                    Data = historico.DataHora == default ? historico.DateCreated : historico.DataHora,
                    Observacao = historico.Observacao ?? string.Empty,
                    Operador = operador
                });
            }

            return resultado;
        }

        private static string MontarNomeCompleto(Utilizador utilizador)
        {
            if (utilizador == null)
                return null;

            var nome = $"{utilizador.FirstName} {utilizador.LastName}".Trim();
            return string.IsNullOrWhiteSpace(nome) ? null : nome;
        }

        private static DateTime ObterDataAngola()
        {
            try
            {
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("Africa/Luanda"));
            }
            catch
            {
                try
                {
                    return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time"));
                }
                catch
                {
                    return DateTime.UtcNow.AddHours(1);
                }
            }
        }
    }
}