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
    public class GerarMovimentoDiarioCommand : IRequest<BaseCommandResponse>
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }

    public class GerarMovimentoDiarioCommandHandler : IRequestHandler<GerarMovimentoDiarioCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReciboService _reciboService;
        private readonly UsuarioLogado _usuarioLogado;
        private readonly ILogger<GerarMovimentoDiarioCommandHandler> _logger;

        public GerarMovimentoDiarioCommandHandler(
            IUnitOfWork unitOfWork,
            IReciboService reciboService,
            UsuarioLogado usuarioLogado,
            ILogger<GerarMovimentoDiarioCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _reciboService = reciboService;
            _usuarioLogado = usuarioLogado;
            _logger = logger;
        }

        public async Task<BaseCommandResponse> Handle(GerarMovimentoDiarioCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("📊 [MOV-DIARIO-{CorrelationId}] Gerando movimento diário para {Data}",
                    correlationId, $"{request.DataInicio:dd/MM/yyyy} a {request.DataFim:dd/MM/yyyy}");

                if (request.DataFim.Date < request.DataInicio.Date)
                    throw new ArgumentException("A data fim não pode ser menor que a data início.");

                var parametros = await _unitOfWork.Parametros.Get(1);
                if (parametros == null)
                    throw new ArgumentException("Parâmetros do sistema não encontrados.");

                var lancamentos = new List<Hotel.Domain.Entities.LancamentoCaixa>();
                var checkins = new List<Checkins>();
                var checkouts = new List<Checkins>();
                var historicos = new List<Hotel.Domain.Entities.Historico>();

                for (var data = request.DataInicio.Date; data <= request.DataFim.Date; data = data.AddDays(1))
                {
                    var caixa = await _unitOfWork.caixa.GetByDateAsync(data, _usuarioLogado.IdUtilizador, _usuarioLogado.perfil);
                    int? caixaId = caixa?.Id;

                    if (!caixaId.HasValue)
                    {
                        _logger.LogWarning("⚠️ [MOV-DIARIO-{CorrelationId}] Nenhum caixa encontrado para {Data}. O dia será processado apenas com o filtro disponível.",
                            correlationId, data.ToString("dd/MM/yyyy"));
                    }

                    lancamentos.AddRange(await _unitOfWork.lancamentoCaixas.GetPagamentosFechamentoCaixaAsync(
                        data,
                        caixaId,
                        _usuarioLogado.Role,
                        _usuarioLogado.UserId));

                    checkins.AddRange(await _unitOfWork.checkins.GetCheckinsFechamentoCaixaAsync(data, caixaId));
                    checkouts.AddRange(await _unitOfWork.checkins.GetCheckoutsFechamentoCaixaAsync(data, caixaId));
                    historicos.AddRange(await _unitOfWork.historico.GetHistoricoFechamentoCaixaAsync(data, caixaId));
                }

                var dto = new MovimentoDiarioDto
                {
                    NomeHotel = parametros.NomeEmpresa,
                    Endereco = parametros.Endereco,
                    Cidade = parametros.Cidade,
                    NumContribuinte = parametros.NumContribuinte,
                    Telefone = parametros.Telefone,
                    LogoCaminho = parametros.LogoCaminho,
                    DataInicio = request.DataInicio.Date,
                    DataFim = request.DataFim.Date,
                    DataRelatorio = request.DataInicio.Date,
                    DataImpressao = ObterDataAngola(),
                    Pagamentos = MapearPagamentos(lancamentos),
                    Checkins = MapearCheckins(checkins, useCheckin: true),
                    Checkouts = MapearCheckins(checkouts, useCheckin: false),
                    Historicos = MapearHistoricos(historicos)
                };

                var pdfBytes = _reciboService.GerarMovimentoDiario(dto);

                response.Success = true;
                response.Message = "Movimento diário gerado com sucesso";
                response.Data = new
                {
                    pdf = Convert.ToBase64String(pdfBytes),
                    nomeArquivo = $"Movimento_Diario_{request.DataInicio:yyyyMMdd}_a_{request.DataFim:yyyyMMdd}.pdf",
                    dataInicio = request.DataInicio.ToString("dd/MM/yyyy"),
                    dataFim = request.DataFim.ToString("dd/MM/yyyy")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [MOV-DIARIO-{CorrelationId}] Erro ao gerar movimento diário", correlationId);
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

        private List<MovimentoPagamentoDto> MapearPagamentos(IEnumerable<Hotel.Domain.Entities.LancamentoCaixa> lancamentos)
        {
            var lista = new List<MovimentoPagamentoDto>();
            foreach (var l in lancamentos)
            {
                var operador = MontarNome(l.Utilizadores) ?? "-";
                var forma = l.TipoPagamentos?.Descricao ?? "-";
                var observacao = l.Pagamentos?.Observacao ?? l.Observacao ?? "-";

                lista.Add(new MovimentoPagamentoDto
                {
                    FormaPagamento = forma,
                    Data = l.DataHoraLancamento,
                    Observacao = observacao,
                    Operador = operador,
                    Entradas = l.TipoLancamento == Hotel.Domain.Enums.TipoLancamento.E ? l.Valor : 0f,
                    Saidas = l.TipoLancamento == Hotel.Domain.Enums.TipoLancamento.S ? l.Valor : 0f
                });
            }
            return lista;
        }

        private List<MovimentoCheckinDto> MapearCheckins(IEnumerable<Checkins> checkins, bool useCheckin)
        {
            var lista = new List<MovimentoCheckinDto>();
            foreach (var c in checkins)
            {
                var hospedagem = c.Hospedagem?.FirstOrDefault();
                var quarto = hospedagem?.Apartamentos?.Codigo ?? "-";
                var periodo = hospedagem != null
                    ? $"{hospedagem.DataAbertura:dd/MM/yyyy} a {hospedagem.PrevisaoFechamento:dd/MM/yyyy} ({hospedagem.QuantidadeDeDiarias} diária{(hospedagem.QuantidadeDeDiarias != 1 ? "s" : "")})"
                    : "-";

                var hospede = c.Hospedes?.FirstOrDefault();
                var nomeHospede = hospede?.Clientes?.Nome ?? "PASSANTE";

                var empresa = "CONTA PROPRIA";
                if (hospede != null && hospede.Estado != Hotel.Domain.Entities.Hospede.EstadoHospede.ContaPropria)
                    empresa = hospedagem?.Empresas?.RazaoSocial ?? "N/D";

                var utilizador = useCheckin
                    ? (MontarNome(c.UtilizadoresCheckin) ?? "-")
                    : (MontarNome(c.UtilizadoresCheckout) ?? "-");

                lista.Add(new MovimentoCheckinDto
                {
                    CheckinId = c.Id,
                    Quarto = quarto,
                    Periodo = periodo,
                    Hospede = nomeHospede,
                    Empresa = empresa,
                    Utilizador = utilizador
                });
            }
            return lista;
        }

        private List<MovimentoHistoricoDto> MapearHistoricos(IEnumerable<Hotel.Domain.Entities.Historico> historicos)
        {
            var lista = new List<MovimentoHistoricoDto>();
            foreach (var h in historicos)
            {
                lista.Add(new MovimentoHistoricoDto
                {
                    Numero = h.Id,
                    DataHora = h.DataHora,
                    Observacao = ObterObservacaoHistorico(h),
                    Utilizador = MontarNome(h.Utilizadores) ?? h.Utilizadores?.UserName ?? "-"
                });
            }
            return lista;
        }

        private static string ObterObservacaoHistorico(Hotel.Domain.Entities.Historico historico)
        {
            if (!string.IsNullOrWhiteSpace(historico?.Observacao))
                return historico.Observacao;

            if (!string.IsNullOrWhiteSpace(historico?.Checkins?.Observacao))
                return historico.Checkins.Observacao;

            return "-";
        }

        private static string MontarNome(Utilizador utilizador)
        {
            if (utilizador == null) return null;
            var nome = $"{utilizador.FirstName} {utilizador.LastName}".Trim();
            if (!string.IsNullOrWhiteSpace(nome)) return nome;
            if (!string.IsNullOrWhiteSpace(utilizador.UserName)) return utilizador.UserName;
            if (!string.IsNullOrWhiteSpace(utilizador.Email)) return utilizador.Email;
            return null;
        }
    }
}
