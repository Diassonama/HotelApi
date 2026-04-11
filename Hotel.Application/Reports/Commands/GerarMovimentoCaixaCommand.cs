using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hotel.Application.Dtos;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Reports.Commands
{
    public class GerarMovimentoCaixaCommand : IRequest<BaseCommandResponse>
    {
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Perfil { get; set; }
        public string Usuario { get; set; }
    }

    public class GerarMovimentoCaixaCommandHandler : IRequestHandler<GerarMovimentoCaixaCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReciboService _reciboService;
        private readonly ILogger<GerarMovimentoCaixaCommandHandler> _logger;

        public GerarMovimentoCaixaCommandHandler(
            IUnitOfWork unitOfWork,
            IReciboService reciboService,
            ILogger<GerarMovimentoCaixaCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _reciboService = reciboService;
            _logger = logger;
        }

        public async Task<BaseCommandResponse> Handle(GerarMovimentoCaixaCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("💰 [MOV-CAIXA-{CorrelationId}] Gerando movimento de caixa para período {DataInicio} a {DataFim}",
                    correlationId, request.DataInicio?.ToString("dd/MM/yyyy") ?? "N/A", request.DataFim?.ToString("dd/MM/yyyy") ?? "N/A");

                var parametros = await _unitOfWork.Parametros.Get(1);
                if (parametros == null)
                    throw new ArgumentException("Parâmetros do sistema não encontrados.");

                var lancamentos = await _unitOfWork.lancamentoCaixas.GetMovimentoCaixaAsync(
                    request.DataInicio, 
                    request.DataFim, 
                    request.Perfil, 
                    request.Usuario);

                // Montar período para exibição
                var dataInício = request.DataInicio ?? DateTime.Now;
                var dataFim = request.DataFim ?? DateTime.Now;
                var periodo = $"{dataInício:dd/MM/yyyy HH:mm:ss} a {dataFim:dd/MM/yyyy HH:mm:ss}";

                var dto = new MovimentoCaixaDto
                {
                    NomeHotel = parametros.NomeEmpresa,
                    Endereco = parametros.Endereco,
                    Cidade = parametros.Cidade,
                    NumContribuinte = parametros.NumContribuinte,
                    Telefone = parametros.Telefone,
                    LogoCaminho = parametros.LogoCaminho,
                    DataInicio = request.DataInicio,
                    DataFim = request.DataFim,
                    DataRelatorio = request.DataInicio ?? DateTime.Now,
                    DataImpressao = ObterDataAngola(),
                    Periodo = periodo,
                    UsuarioFiltrado = request.Usuario ?? "TODOS",
                    Linhas = MapearLinhas(lancamentos),
                    TotalEntradas = lancamentos.Where(l => l.TipoLancamento == Hotel.Domain.Enums.TipoLancamento.E).Sum(l => l.Valor),
                    TotalSaidas = lancamentos.Where(l => l.TipoLancamento == Hotel.Domain.Enums.TipoLancamento.S).Sum(l => l.Valor)
                };

                var pdfBytes = _reciboService.GerarMovimentoCaixa(dto);

                response.Success = true;
                response.Message = "Movimento de caixa gerado com sucesso";
                response.Data = new
                {
                    pdf = Convert.ToBase64String(pdfBytes),
                    nomeArquivo = $"Movimento_Caixa_{request.DataInicio:yyyyMMdd}_a_{request.DataFim:yyyyMMdd}.pdf",
                    periodo = periodo,
                    totalRegistros = lancamentos.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [MOV-CAIXA-{CorrelationId}] Erro ao gerar movimento de caixa", correlationId);
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private DateTime ObterDataAngola()
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

        private List<MovimentoCaixaLinhaDto> MapearLinhas(List<Hotel.Domain.Entities.LancamentoCaixa> lancamentos)
        {
            var lista = new List<MovimentoCaixaLinhaDto>();
            foreach (var l in lancamentos)
            {
                lista.Add(new MovimentoCaixaLinhaDto
                {
                    Data = l.DataHoraLancamento.ToString("dd-MM-yyyy"),
                    FormaPagamento = l.TipoPagamentos?.Descricao ?? "-",
                    Observacao = !string.IsNullOrWhiteSpace(l.Observacao)
                        ? l.Observacao
                        : (!string.IsNullOrWhiteSpace(l.PlanodeContas?.Descricao)
                            ? l.PlanodeContas.Descricao
                            : (l.Pagamentos?.Observacao ?? "-")),
                    Operador = MontarNome(l.Utilizadores) ?? "Sistema",
                    Entradas = l.TipoLancamento == Hotel.Domain.Enums.TipoLancamento.E ? l.Valor : 0f,
                    Saidas = l.TipoLancamento == Hotel.Domain.Enums.TipoLancamento.S ? l.Valor : 0f
                });
            }
            return lista;
        }

        private static string MontarNome(Utilizador utilizador)
        {
            if (utilizador == null) return null;
            var nome = $"{utilizador.FirstName} {utilizador.LastName}".Trim();
            return string.IsNullOrWhiteSpace(nome) ? null : nome;
        }
    }
}
