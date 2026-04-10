using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hotel.Application.DTOs;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Domain.Dtos;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Reports.Commands
{
    public class GerarGovernancaArrumacaoCommand : IRequest<BaseCommandResponse>
    {
        public string Titulo { get; set; }
    }

    public class GerarGovernancaArrumacaoCommandHandler : IRequestHandler<GerarGovernancaArrumacaoCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReciboService _reciboService;
        private readonly ILogger<GerarGovernancaArrumacaoCommandHandler> _logger;

        public GerarGovernancaArrumacaoCommandHandler(
            IUnitOfWork unitOfWork,
            IReciboService reciboService,
            ILogger<GerarGovernancaArrumacaoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _reciboService = reciboService;
            _logger = logger;
        }

        public async Task<BaseCommandResponse> Handle(GerarGovernancaArrumacaoCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("🧹 [GOV-ARRUMACAO-{CorrelationId}] Gerando relatório de governança/arrumação", correlationId);

                var ocupados = await _unitOfWork.Apartamento.GetApartamentosComCheckinAtivoAsync();
                var linhas = (ocupados ?? new List<ApartamentoComCheckinAtivoDto>())
                    .OrderBy(x => x.Apartamento?.Codigo)
                    .Select(MapearLinha)
                    .ToList();

                var pdfBytes = _reciboService.GerarRelatorioGovernancaArrumacao(
                    linhas,
                    request.Titulo ?? "GOVERNANÇA RELATÓRIO DE ARRUMAÇÃO");

                response.Success = true;
                response.Message = "Relatório de governança gerado com sucesso";
                response.Data = new
                {
                    pdf = Convert.ToBase64String(pdfBytes),
                    nomeArquivo = $"Governanca_Arrumacao_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                    totalRegistros = linhas.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GOV-ARRUMACAO-{CorrelationId}] Erro ao gerar relatório", correlationId);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static GovernancaArrumacaoDto MapearLinha(ApartamentoComCheckinAtivoDto item)
        {
            var hospedagem = item.Hospedagem;
            var checkin = item.Checkin;
            var apartamento = item.Apartamento;
            var hospede = item.Hospede;

            var nomeHospede = hospede?.Clientes?.Nome;
            if (string.IsNullOrWhiteSpace(nomeHospede))
                nomeHospede = "Passante";

            var tipo = apartamento?.TipoApartamentos?.Descricao?.ToUpperInvariant() ?? string.Empty;
            var pax = checkin?.Hospedes?.Count ?? 0;
            if (pax <= 0 && hospedagem != null)
                pax = hospedagem.QuantidadeHomens + hospedagem.QuantidadeMulheres + hospedagem.QuantidadeCrianca;
            if (pax <= 0)
                pax = 1;

            var dormiuFora = hospedagem != null && hospedagem.DataAbertura.Date < DateTime.Today;

            return new GovernancaArrumacaoDto
            {
                CheckinId = checkin?.Id ?? 0,
                Codigo = apartamento?.Codigo ?? string.Empty,
                Hospede = nomeHospede,
                Tipo = tipo,
                Pax = pax,
                Checkin = checkin?.DataEntrada ?? DateTime.Today,
                Checkout = hospedagem?.PrevisaoFechamento ?? DateTime.Today,
                PaxGov = string.Empty,
                Limpo = string.Empty,
                PV = apartamento != null && apartamento.NaoPertube ? "X" : string.Empty,
                NQA = string.Empty,
                DF = dormiuFora ? "X" : string.Empty,
                SB = string.Empty,
                MB = string.Empty,
                PB = string.Empty,
                Observacao = apartamento?.Observacao ?? string.Empty
            };
        }
    }
}