using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hotel.Application.DTOs;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Reports.Commands
{
    public class GerarVistoriaQuartosCommand : IRequest<BaseCommandResponse>
    {
        public string Titulo { get; set; }
    }

    public class GerarVistoriaQuartosCommandHandler : IRequestHandler<GerarVistoriaQuartosCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReciboService _reciboService;
        private readonly UsuarioLogado _usuarioLogado;
        private readonly ILogger<GerarVistoriaQuartosCommandHandler> _logger;

        public GerarVistoriaQuartosCommandHandler(
            IUnitOfWork unitOfWork,
            IReciboService reciboService,
            UsuarioLogado usuarioLogado,
            ILogger<GerarVistoriaQuartosCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _reciboService = reciboService;
            _usuarioLogado = usuarioLogado;
            _logger = logger;
        }

        public async Task<BaseCommandResponse> Handle(GerarVistoriaQuartosCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("📝 [VISTORIA-QUARTOS-{CorrelationId}] Gerando relatório de vistoria", correlationId);

                var apartamentos = (await _unitOfWork.Apartamento.GetApartamentoAsync())?.ToList() ?? new List<Hotel.Domain.Entities.Apartamentos>();
                var linhas = apartamentos
                    .OrderBy(x => x.Codigo)
                    .Select(MapearLinha)
                    .ToList();

                var funcionario = ObterNomeFuncionario();
                var dataReferencia = DateTime.Now;
                var pdfBytes = _reciboService.GerarRelatorioVistoriaQuartos(
                    linhas,
                    request.Titulo ?? "Vistória de Quartos",
                    funcionario,
                    dataReferencia);

                response.Success = true;
                response.Message = "Relatório de vistoria gerado com sucesso";
                response.Data = new
                {
                    pdf = Convert.ToBase64String(pdfBytes),
                    nomeArquivo = $"Vistoria_Quartos_{DateTime.Now:yyyyMMdd_HHmm}.pdf",
                    totalRegistros = linhas.Count,
                    funcionario,
                    data = dataReferencia
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [VISTORIA-QUARTOS-{CorrelationId}] Erro ao gerar relatório", correlationId);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private VistoriaQuartoDto MapearLinha(Hotel.Domain.Entities.Apartamentos apartamento)
        {
            var pax = apartamento.checkins?.Hospedes?.Count ?? 0;
            if (pax <= 0)
            {
                var hospedagemAtiva = apartamento.checkins?.Hospedagem?
                    .Where(x => x.DataFechamento == null)
                    .OrderByDescending(x => x.DataAbertura)
                    .FirstOrDefault();

                if (hospedagemAtiva != null)
                    pax = hospedagemAtiva.QuantidadeHomens + hospedagemAtiva.QuantidadeMulheres + hospedagemAtiva.QuantidadeCrianca;
            }

            return new VistoriaQuartoDto
            {
                Quarto = apartamento.Codigo,
                TipoQuarto = apartamento.TipoApartamentos?.Descricao ?? string.Empty,
                Situacao = MapearSituacao(apartamento.Situacao),
                Pax = pax,
                Observacao = apartamento.Observacao ?? string.Empty
            };
        }

        private string ObterNomeFuncionario()
        {
            return _usuarioLogado.UserName
                ?? _usuarioLogado.Utilizador
                ?? _usuarioLogado.UserId
                ?? string.Empty;
        }

        private static string MapearSituacao(Situacao situacao)
        {
            return situacao switch
            {
                Situacao.Livre => "Livre",
                Situacao.Ocupado => "Ocupado",
                Situacao.Manuntencao => "Manutenção",
                Situacao.Atrasado => "Atrasado",
                Situacao.Hoje => "Hoje",
                Situacao.Amanha => "Amanha",
                Situacao.Limpeza => "Limpeza",
                Situacao.Bloqueado => "Bloqueado",
                Situacao.Reservado => "Reservado",
                _ => "Indefinido"
            };
        }
    }
}