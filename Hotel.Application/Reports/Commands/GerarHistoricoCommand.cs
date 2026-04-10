
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hotel.Application.Dtos;
using Hotel.Application.DTOs;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Reports.Commands
{
    public class GerarHistoricoCommand : IRequest<BaseCommandResponse>
    {
     //   public int EmpresaId { get; set; }
        public string Titulo { get; set; }
    }

    public class GerarHistoricoCommandHandler : IRequestHandler<GerarHistoricoCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReciboService _reciboService;
        private readonly ILogger<GerarHistoricoCommandHandler> _logger;

        public GerarHistoricoCommandHandler(
            IUnitOfWork unitOfWork,
            IReciboService reciboService,
            ILogger<GerarHistoricoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _reciboService = reciboService;
            _logger = logger;
        }

        public async Task<BaseCommandResponse> Handle(GerarHistoricoCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

       
    try
    {
        _logger.LogInformation("📊 [GERAR-HISTORICO-{CorrelationId}] Gerando histórico de ocupação",
            correlationId);

        var totalQuartos = _unitOfWork.Apartamento.GetAll().Count();

        // ✅ BUSCAR APENAS APARTAMENTOS COM CHECKIN/HOSPEDAGEM ATIVA
        var apartamentosOcupados = await _unitOfWork.Apartamento.GetApartamentosComCheckinAtivoAsync();
        if (apartamentosOcupados == null)
            apartamentosOcupados = new List<ApartamentoComCheckinAtivoDto>();

        _logger.LogInformation("📝 [GERAR-HISTORICO-{CorrelationId}] Total de apartamentos: {Total}, Ocupados agora: {Ocupados}",
            correlationId, totalQuartos, apartamentosOcupados.Count);

        // ✅ CONSTRUIR LISTA DE ITEMS PARA O RELATÓRIO
        var linhas = new List<HistoricoOcupacaoDto>();

        foreach (var item in apartamentosOcupados.OrderBy(a => a.Apartamento?.Codigo))
        {
            try
            {
                var hospedagem = item.Hospedagem;
                var checkin = item.Checkin;
                var hospede = item.Hospede;
                var apartamento = item.Apartamento;

                if (hospedagem == null || checkin == null || apartamento == null)
                    continue;

                var statusCheckout = DeterminarStatusCheckout(hospedagem.PrevisaoFechamento);
                var nomeHospede = hospede?.Clientes?.Nome;
                if (string.IsNullOrWhiteSpace(nomeHospede))
                    nomeHospede = "Passante";

                var nomeEmpresa = item.Empresa?.RazaoSocial;
                if (string.IsNullOrWhiteSpace(nomeEmpresa))
                    nomeEmpresa = "Conta Propria";

                linhas.Add(new HistoricoOcupacaoDto
                {
                    Checkin = checkin.Id,
                    Quarto = apartamento.Codigo,
                    Hospede = nomeHospede,
                    Empresa = nomeEmpresa,
                    DataAbertura = hospedagem.DataAbertura,
                    Checkout = statusCheckout
                });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️  [GERAR-HISTORICO-{CorrelationId}] Erro ao processar: {Message}",
                    correlationId, ex.Message);
                continue;
            }
        }

        var quartosOcupados = apartamentosOcupados.Count;
        var quartosLivres = totalQuartos - quartosOcupados;

        _logger.LogInformation("✅ [GERAR-HISTORICO-{CorrelationId}] Relatório construído - Total: {Total}, Ocupados: {Ocupados}, Livres: {Livres}",
            correlationId, totalQuartos, quartosOcupados, quartosLivres);

        // ✅ GERAR PDF
        var pdfBytes = _reciboService.GerarHistoricoOcupacao(
            linhas,
            request.Titulo ?? "Histórico de Ocupação",
            totalQuartos,
            quartosOcupados
        );

        _logger.LogInformation("✅ [GERAR-HISTORICO-{CorrelationId}] PDF gerado com sucesso - Tamanho: {TamanhoPDF} bytes",
            correlationId, pdfBytes.Length);

        response.Success = true;
        response.Message = "Histórico de ocupação gerado com sucesso";
        response.Data = new
        {
            pdf = Convert.ToBase64String(pdfBytes),
            nomeArquivo = $"Historico_Ocupacao_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
            totalQuartos = totalQuartos,
            quartosOcupados = quartosOcupados,
            quartosLivres = quartosLivres
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "❌ [GERAR-HISTORICO-{CorrelationId}] Erro ao gerar histórico: {Message}",
            correlationId, ex.Message);

        response.Success = false;
        response.Message = $"Erro ao gerar histórico: {ex.Message}";
    }

            return response;
        }

        private string DeterminarStatusCheckout(DateTime previsaoFechamento)
        {
            var hoje = DateTime.Now.Date;
            var dataFechamento = previsaoFechamento.Date;
            var diferenca = (dataFechamento - hoje).Days;

            return diferenca switch
            {
                0 => "Hoje",
                1 => "Amanha",
                < 0 => "Check out Atrasado",
                _ => $"Em {diferenca} dia(s)"
            };
        }
    }
}