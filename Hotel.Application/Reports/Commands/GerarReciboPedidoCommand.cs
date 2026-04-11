using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hotel.Application.DTOs;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Reports.Commands
{
    public class GerarReciboPedidoCommand : IRequest<BaseCommandResponse>
    {
        public int PedidoId { get; set; }
    }

    public class GerarReciboPedidoCommandHandler : IRequestHandler<GerarReciboPedidoCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReciboService _reciboService;
        private readonly UsuarioLogado _logado;
        private readonly ILogger<GerarReciboPedidoCommandHandler> _logger;

        public GerarReciboPedidoCommandHandler(
            IUnitOfWork unitOfWork,
            IReciboService reciboService,
            UsuarioLogado logado,
            ILogger<GerarReciboPedidoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _reciboService = reciboService;
            _logado = logado;
            _logger = logger;
        }

        public async Task<BaseCommandResponse> Handle(GerarReciboPedidoCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("📄 [RECIBO-PEDIDO-{CorrelationId}] Gerando recibo para PedidoId: {PedidoId}",
                    correlationId, request.PedidoId);

                // ── Pedido com itens, hospede e ponto de venda ───────────────────
                var pedido = await _unitOfWork.Pedidos.GetByIdWithItemsAsync(request.PedidoId);
                if (pedido == null)
                    throw new ArgumentException($"Pedido com ID {request.PedidoId} não encontrado.");

                // ── Parâmetros do hotel ──────────────────────────────────────────
                var parametros = await _unitOfWork.Parametros.Get(1);
                if (parametros == null)
                    throw new ArgumentException("Parâmetros do sistema não encontrados.");

                // ── Pagamento do pedido (último pagamento com origem no pedido) ──
                // Usamos o lancamentoCaixas para obter a forma de pagamento
                string formaPagamento = null;
                string nomeOperador = _logado.IdUtilizador;

                var pagamentos = await _unitOfWork.pagamentos.GetAllByCheckinIdAsync(pedido.IdCheckin);
                var pagamentoPedido = pagamentos?
                    .Where(p => p.OrigemId == pedido.Id)
                    .OrderByDescending(p => p.DataVencimento)
                    .FirstOrDefault();

                if (pagamentoPedido != null)
                {
                    var lancamento = await _unitOfWork.lancamentoCaixas.GetByPagamentoIdAsync(pagamentoPedido.Id);
                    if (lancamento?.TipoPagamentos != null)
                        formaPagamento = lancamento.TipoPagamentos.Descricao;
                    if (!string.IsNullOrEmpty(lancamento?.UtilizadoresId))
                        nomeOperador = await _unitOfWork.Utilizadores.GetNomeCompletoByIdAsync(lancamento.UtilizadoresId);
                }

                // ── Quarto (apenas se hóspede associado) ────────────────────────
                string apartamentoCodigo = null;
                if (pedido.HospedeId > 0)
                {
                    var hospedagem = await _unitOfWork.Hospedagem.GetByCheckinIdAsync(pedido.IdCheckin);
                    apartamentoCodigo = hospedagem?.Apartamentos?.Codigo;
                }

                // ── Itens com nome do produto ────────────────────────────────────
                var itens = pedido.ItemPedidos?.Select(i => new ReciboPedidoItemDto
                {
                    Descricao = i.Produto?.Nome ?? $"Produto #{i.ProdutoId}",
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.Preco,
                    ValorTotal = i.ValorTotal
                }).ToList() ?? new();

                // ── Montar DTO ───────────────────────────────────────────────────
                var dto = new ReciboPedidoDto
                {
                    NomeHotel       = parametros.NomeEmpresa,
                    Endereco        = parametros.Endereco,
                    Cidade          = parametros.Cidade,
                    NumContribuinte = parametros.NumContribuinte,

                    NumePedido      = pedido.NumePedido,
                    DataPedido      = pedido.DataPedido,
                    PontoVendaNome  = pedido.PontoVenda?.Nome ?? string.Empty,
                    Observacao      = pedido.Observacao,

                    NomeCliente     = pedido.Hospede?.Clientes?.Nome ?? "Cliente Diverso",
                    ApartamentoCodigo = apartamentoCodigo,

                    Itens           = itens,
                    ValorTotal      = pedido.ValorTotal,
                    ValorPago       = pedido.ValorTotal,
                    FormaPagamento  = formaPagamento,
                    Operador        = nomeOperador
                };

                _logger.LogInformation("🖨️ [RECIBO-PEDIDO-{CorrelationId}] Gerando PDF — Pedido {NumePedido}, {QtdItens} item(s), Total: {Total}",
                    correlationId, dto.NumePedido, dto.Itens.Count, dto.ValorTotal);

                var pdfBytes = _reciboService.GerarReciboPedido(dto);

                _logger.LogInformation("✅ [RECIBO-PEDIDO-{CorrelationId}] PDF gerado — {Bytes} bytes",
                    correlationId, pdfBytes.Length);

                response.Success = true;
                response.Message = "Recibo do pedido gerado com sucesso.";
                response.Data = new
                {
                    pdf          = Convert.ToBase64String(pdfBytes),
                    nomeArquivo  = $"Recibo_Pedido_{pedido.NumePedido}.pdf",
                    pedidoId     = pedido.Id,
                    numePedido   = pedido.NumePedido
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [RECIBO-PEDIDO-{CorrelationId}] Erro ao gerar recibo PedidoId {PedidoId}: {Message}",
                    correlationId, request.PedidoId, ex.Message);

                response.Success = false;
                response.Message = $"Erro ao gerar recibo: {ex.Message}";
            }

            return response;
        }
    }
}
