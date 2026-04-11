using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.DTOs.Pedido;
using Hotel.Application.Pagamento;
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PedidoController : ApiControllerBase
    {
        private readonly ILogger<PedidoController> _logger;

        public PedidoController(IUnitOfWork unitOfWork, ILogger<PedidoController> logger) : base(unitOfWork)
        {
            _logger = logger;
        }

        /// <summary>
        /// Buscar pedido por ID com itens
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetById(int id)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            try
            {
                _logger.LogInformation("🔍 [PEDIDO-GET-{CorrelationId}] Buscando pedido ID: {Id}", correlationId, id);

                var pedido = await _unitOfWork.Pedidos.GetByIdWithItemsAsync(id);
                if (pedido == null)
                {
                    _logger.LogWarning("⚠️ [PEDIDO-GET-{CorrelationId}] Pedido ID {Id} não encontrado.", correlationId, id);
                    return NotFound(new { mensagem = $"Pedido com ID {id} não encontrado." });
                }

                _logger.LogInformation("✅ [PEDIDO-GET-{CorrelationId}] Pedido {Id} encontrado. Situação: {Situacao}, Valor: {Valor}, Itens: {QtdItens}",
                    correlationId, id, pedido.SituacaoPagamento, pedido.ValorTotal, pedido.QuantidadeItens);

                return Ok(MapToDto(pedido));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PEDIDO-GET-{CorrelationId}] Erro ao buscar pedido ID: {Id}", correlationId, id);
                return StatusCode(500, new { mensagem = "Erro interno ao buscar pedido." });
            }
        }

        /// <summary>
        /// Listar pedidos por check-in
        /// </summary>
        [HttpGet("checkin/{checkinId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetByCheckin(int checkinId)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            try
            {
                _logger.LogInformation("🔍 [PEDIDO-LIST-{CorrelationId}] Buscando pedidos do checkin: {CheckinId}", correlationId, checkinId);

                var pedidos = await _unitOfWork.Pedidos.GetByCheckinIdAsync(checkinId);
                var lista = pedidos.Select(MapToListDto).ToList();

                _logger.LogInformation("✅ [PEDIDO-LIST-{CorrelationId}] {Count} pedido(s) encontrado(s) para checkin {CheckinId}", correlationId, lista.Count, checkinId);

                return Ok(lista);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PEDIDO-LIST-{CorrelationId}] Erro ao listar pedidos do checkin: {CheckinId}", correlationId, checkinId);
                return StatusCode(500, new { mensagem = "Erro interno ao listar pedidos." });
            }
        }

        /// <summary>
        /// Criar novo pedido
        /// </summary>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Criar([FromBody] CreatePedidoDto dto)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            try
            {
                _logger.LogInformation("📝 [PEDIDO-CREATE-{CorrelationId}] Criando pedido para checkin: {CheckinId}", correlationId, dto.IdCheckin);

                var idCaixaAtual = await _unitOfWork.caixa.getCaixa();
                if (idCaixaAtual <= 0)
                {
                    return BadRequest(new { mensagem = "Nenhum caixa atual aberto foi encontrado no sistema." });
                }

                var pedido = new Pedido(idCaixaAtual, dto.IdCheckin, dto.HospedeId, dto.PontoVendaId, dto.Observacao);

                if (dto.Itens != null)
                {
                    foreach (var item in dto.Itens)
                    {
                        pedido.AdicionarItem(item.ProdutoId, item.Preco, item.Quantidade);
                    }
                }

                await _unitOfWork.Pedidos.Add(pedido);
                await _unitOfWork.Save();

                _logger.LogInformation("✅ [PEDIDO-CREATE-{CorrelationId}] Pedido criado com ID: {PedidoId}", correlationId, pedido.Id);

                return CreatedAtAction(nameof(GetById), new { id = pedido.Id }, new
                {
                    id = pedido.Id,
                    numePedido = pedido.NumePedido,
                    mensagem = "Pedido criado com sucesso."
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("⚠️ [PEDIDO-CREATE-{CorrelationId}] Dados inválidos: {Erro}", correlationId, ex.Message);
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PEDIDO-CREATE-{CorrelationId}] Erro ao criar pedido", correlationId);
                return StatusCode(500, new { mensagem = "Erro interno ao criar pedido." });
            }
        }

        /// <summary>
        /// Adicionar item ao pedido
        /// </summary>
        [HttpPost("{id:int}/itens")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AdicionarItem(int id, [FromBody] AddItemPedidoDto dto)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            try
            {
                _logger.LogInformation("➕ [PEDIDO-ADD-ITEM-{CorrelationId}] Adicionando item ProdutoId:{ProdutoId} (Qtd:{Qtd}, Preço:{Preco}) ao pedido {PedidoId}",
                    correlationId, dto.ProdutoId, dto.Quantidade, dto.Preco, id);

                var pedido = await _unitOfWork.Pedidos.GetByIdWithItemsAsync(id);
                if (pedido == null)
                    return NotFound(new { mensagem = $"Pedido com ID {id} não encontrado." });

                pedido.AdicionarItem(dto.ProdutoId, dto.Preco, dto.Quantidade);

                await _unitOfWork.Pedidos.Update(pedido);
                await _unitOfWork.Save();

                _logger.LogInformation("✅ [PEDIDO-ADD-ITEM-{CorrelationId}] Item adicionado ao pedido {PedidoId}", correlationId, id);

                return Ok(new { mensagem = "Item adicionado com sucesso.", valorTotal = pedido.ValorTotal });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PEDIDO-ADD-ITEM-{CorrelationId}] Erro ao adicionar item ao pedido {PedidoId}", correlationId, id);
                return StatusCode(500, new { mensagem = "Erro interno ao adicionar item." });
            }
        }

        /// <summary>
        /// Remover item do pedido
        /// </summary>
        [HttpDelete("{id:int}/itens/{produtoId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RemoverItem(int id, int produtoId)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            try
            {
                _logger.LogInformation("🗑️ [PEDIDO-REMOVE-ITEM-{CorrelationId}] Removendo ProdutoId:{ProdutoId} do pedido {PedidoId}", correlationId, produtoId, id);

                var pedido = await _unitOfWork.Pedidos.GetByIdWithItemsAsync(id);
                if (pedido == null)
                    return NotFound(new { mensagem = $"Pedido com ID {id} não encontrado." });

                pedido.RemoverItem(produtoId);

                await _unitOfWork.Pedidos.Update(pedido);
                await _unitOfWork.Save();

                _logger.LogInformation("✅ [PEDIDO-REMOVE-ITEM-{CorrelationId}] Item {ProdutoId} removido do pedido {PedidoId}", correlationId, produtoId, id);

                return Ok(new { mensagem = "Item removido com sucesso.", valorTotal = pedido.ValorTotal });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PEDIDO-REMOVE-ITEM-{CorrelationId}] Erro ao remover item do pedido {PedidoId}", correlationId, id);
                return StatusCode(500, new { mensagem = "Erro interno ao remover item." });
            }
        }

        /// <summary>
        /// Atualizar quantidade de um item do pedido
        /// </summary>
        [HttpPut("{id:int}/itens/quantidade")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AtualizarQuantidade(int id, [FromBody] UpdateQuantityItemDto dto)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            try
            {
                _logger.LogInformation("✏️ [PEDIDO-UPDATE-QTY-{CorrelationId}] Atualizando qtd ProdutoId:{ProdutoId} → {NovaQtd} no pedido {PedidoId}",
                    correlationId, dto.ProdutoId, dto.NovaQuantidade, id);

                var pedido = await _unitOfWork.Pedidos.GetByIdWithItemsAsync(id);
                if (pedido == null)
                    return NotFound(new { mensagem = $"Pedido com ID {id} não encontrado." });

                pedido.AlterarQuantidadeItem(dto.ProdutoId, dto.NovaQuantidade);

                await _unitOfWork.Pedidos.Update(pedido);
                await _unitOfWork.Save();

                return Ok(new { mensagem = "Quantidade atualizada com sucesso.", valorTotal = pedido.ValorTotal });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PEDIDO-UPDATE-QTY-{CorrelationId}] Erro ao atualizar quantidade no pedido {PedidoId}", correlationId, id);
                return StatusCode(500, new { mensagem = "Erro interno ao atualizar quantidade." });
            }
        }

        /// <summary>
        /// Atualizar observação do pedido
        /// </summary>
        [HttpPut("{id:int}/observacao")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AtualizarObservacao(int id, [FromBody] UpdateObservacaoDto dto)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            try
            {
                _logger.LogInformation("📝 [PEDIDO-UPDATE-OBS-{CorrelationId}] Atualizando observação do pedido {PedidoId}", correlationId, id);

                var pedido = await _unitOfWork.Pedidos.GetByIdWithItemsAsync(id);
                if (pedido == null)
                    return NotFound(new { mensagem = $"Pedido com ID {id} não encontrado." });

                pedido.AtualizarObservacao(dto.Observacao);

                await _unitOfWork.Pedidos.Update(pedido);
                await _unitOfWork.Save();

                return Ok(new { mensagem = "Observação atualizada com sucesso." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PEDIDO-UPDATE-OBS-{CorrelationId}] Erro ao atualizar observação do pedido {PedidoId}", correlationId, id);
                return StatusCode(500, new { mensagem = "Erro interno ao atualizar observação." });
            }
        }

        /// <summary>
        /// Confirmar pagamento do pedido
        /// </summary>
        [HttpPut("{id:int}/confirmar-pagamento")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ConfirmarPagamento(int id, [FromBody] ConfirmarPagamentoPedidoDto dto)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            try
            {
                _logger.LogInformation("💳 [PEDIDO-CONFIRMAR-{CorrelationId}] Confirmando pagamento do pedido {PedidoId}", correlationId, id);

                if (dto == null)
                    return BadRequest(new { mensagem = "Dados do pagamento não informados." });

                _logger.LogDebug("🔍 [PEDIDO-CONFIRMAR-{CorrelationId}] Buscando pedido {PedidoId} com itens...", correlationId, id);
                var pedido = await _unitOfWork.Pedidos.GetByIdWithItemsAsync(id);
                if (pedido == null)
                {
                    _logger.LogWarning("⚠️ [PEDIDO-CONFIRMAR-{CorrelationId}] Pedido {PedidoId} não encontrado.", correlationId, id);
                    return NotFound(new { mensagem = $"Pedido com ID {id} não encontrado." });
                }

                _logger.LogInformation("📦 [PEDIDO-CONFIRMAR-{CorrelationId}] Pedido {PedidoId} carregado — Situação:{Situacao}, Valor:{Valor}, Itens:{QtdItens}",
                    correlationId, id, pedido.SituacaoPagamento, pedido.ValorTotal, pedido.QuantidadeItens);

                foreach (var item in pedido.ItemPedidos)
                    _logger.LogDebug("   🧾 Item ProdutoId:{ProdutoId} Qtd:{Qtd} Preco:{Preco} Total:{Total}",
                        item.ProdutoId, item.Quantidade, item.Preco, item.ValorTotal);

                if (pedido.PedidoFinalizado)
                    return BadRequest(new { mensagem = "Pedido já foi pago." });

                var valorPagamento = dto.ValorPago ?? pedido.ValorTotal;
                if (valorPagamento <= 0)
                    return BadRequest(new { mensagem = "Valor do pagamento deve ser maior que zero." });

                var pagamentoCommand = new CreatePagamentoCommand
                {
                    pagamentoRequest = new PagamentoRequest
                    {
                        CheckinsId = pedido.IdCheckin,
                        HospedesId = pedido.HospedeId,
                        Valor = (float)valorPagamento,
                        ValorPago = (float)valorPagamento,
                        DataPagamento = DateTime.Now,
                        TipoPagamentosId = dto.TipoPagamentosId,
                        Observacao = dto.Observacao,
                        OrigemId = pedido.Id,
                        Origem = "Restaurante"

                    }
                };

                _logger.LogInformation("💸 [PEDIDO-CONFIRMAR-{CorrelationId}] Enviando pagamento — Valor:{Valor}, TipoPagtoId:{TipoPagtoId}",
                    correlationId, valorPagamento, dto.TipoPagamentosId);

                var respostaPagamento = await Mediator.Send(pagamentoCommand, CancellationToken.None);

                _logger.LogInformation("💳 [PEDIDO-CONFIRMAR-{CorrelationId}] Resposta pagamento — Success:{Success}, Msg:{Msg}",
                    correlationId, respostaPagamento.Success, respostaPagamento.Message);

                if (!respostaPagamento.Success)
                {
                    _logger.LogWarning("⚠️ [PEDIDO-CONFIRMAR-{CorrelationId}] Pagamento recusado: {Msg} | Erros: {Erros}",
                        correlationId, respostaPagamento.Message, string.Join("; ", respostaPagamento.Errors ?? new List<string>()));
                    return BadRequest(new
                    {
                        mensagem = respostaPagamento.Message ?? "Falha ao processar pagamento do pedido.",
                        erros = respostaPagamento.Errors
                    });
                }

                pedido.ConfirmarPagamento();

                await _unitOfWork.Pedidos.Update(pedido);
                await _unitOfWork.Save();

                _logger.LogInformation("✅ [PEDIDO-CONFIRMAR-{CorrelationId}] Pagamento confirmado para pedido {PedidoId}", correlationId, id);

                return Ok(new
                {
                    mensagem = "Pagamento confirmado com sucesso.",
                    numePedido = pedido.NumePedido,
                    valorTotal = pedido.ValorTotal,
                    valorPago = valorPagamento,
                    dataFinalizacao = pedido.DataFinalizacao
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PEDIDO-CONFIRMAR-{CorrelationId}] Erro ao confirmar pagamento do pedido {PedidoId}", correlationId, id);
                return StatusCode(500, new { mensagem = "Erro interno ao confirmar pagamento." });
            }
        }

        /// <summary>
        /// Cancelar pedido
        /// </summary>
        [HttpPut("{id:int}/cancelar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Cancelar(int id)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            try
            {
                _logger.LogInformation("🚫 [PEDIDO-CANCELAR-{CorrelationId}] Cancelando pedido {PedidoId}", correlationId, id);

                var pedido = await _unitOfWork.Pedidos.GetByIdWithItemsAsync(id);
                if (pedido == null)
                    return NotFound(new { mensagem = $"Pedido com ID {id} não encontrado." });

                pedido.CancelarPedido();

                await _unitOfWork.Pedidos.Update(pedido);
                await _unitOfWork.Save();

                _logger.LogInformation("✅ [PEDIDO-CANCELAR-{CorrelationId}] Pedido {PedidoId} cancelado", correlationId, id);

                return Ok(new { mensagem = "Pedido cancelado com sucesso." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PEDIDO-CANCELAR-{CorrelationId}] Erro ao cancelar pedido {PedidoId}", correlationId, id);
                return StatusCode(500, new { mensagem = "Erro interno ao cancelar pedido." });
            }
        }

        /// <summary>
        /// Estornar pagamento do pedido
        /// </summary>
        [HttpPut("{id:int}/estornar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Estornar(int id, [FromBody] EstornoPedidoDto dto)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];
            try
            {
                _logger.LogInformation("↩️ [PEDIDO-ESTORNAR-{CorrelationId}] Estornando pedido {PedidoId}", correlationId, id);

                var pedido = await _unitOfWork.Pedidos.GetByIdWithItemsAsync(id);
                if (pedido == null)
                    return NotFound(new { mensagem = $"Pedido com ID {id} não encontrado." });

                pedido.EstornarPagamento(dto.Motivo);

                await _unitOfWork.Pedidos.Update(pedido);
                await _unitOfWork.Save();

                _logger.LogInformation("✅ [PEDIDO-ESTORNAR-{CorrelationId}] Pedido {PedidoId} estornado", correlationId, id);

                return Ok(new { mensagem = "Estorno realizado com sucesso." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PEDIDO-ESTORNAR-{CorrelationId}] Erro ao estornar pedido {PedidoId}", correlationId, id);
                return StatusCode(500, new { mensagem = "Erro interno ao estornar pedido." });
            }
        }

        // ─── Helpers de mapeamento ──────────────────────────────────────────

        private static PedidoDto MapToDto(Pedido p) => new()
        {
            Id = p.Id,
            IdCaixa = p.IdCaixa,
            IdCheckin = p.IdCheckin,
            HospedeId = p.HospedeId,
            PontoVendaId = p.PontoVendaId,
            NumePedido = p.NumePedido,
            Observacao = p.Observacao,
            DataPedido = p.DataPedido,
            DataFinalizacao = p.DataFinalizacao,
            SituacaoPagamento = (Hotel.Domain.Enums.SituacaoPagamentoPedido)(int)p.SituacaoPagamento,
            SituacaoPagamentoDescricao = p.SituacaoPagamento.ToString(),
            Valor = p.Valor,
            ValorTotal = p.ValorTotal,
            QuantidadeItens = p.QuantidadeItens,
            PedidoFinalizado = p.PedidoFinalizado,
            PodeCancelar = p.PodeCancelar,
            NomeHospede = p.Hospede?.Clientes?.Nome ?? string.Empty,
            PontoVendaNome = p.PontoVenda?.Nome ?? string.Empty,
            Itens = p.ItemPedidos?.Select(i => new ItemPedidoDto
            {
                Id = i.Id,
                ProdutoId = i.ProdutoId,
                Preco = i.Preco,
                Quantidade = i.Quantidade,
                ValorTotal = i.ValorTotal
            }).ToList() ?? new()
        };

        private static PedidoListDto MapToListDto(Pedido p) => new()
        {
            Id = p.Id,
            NumePedido = p.NumePedido,
            NomeHospede = p.Hospede?.Clientes?.Nome ?? string.Empty,
            PontoVendaNome = p.PontoVenda?.Nome ?? string.Empty,
            DataPedido = p.DataPedido,
            SituacaoPagamento = (Hotel.Domain.Enums.SituacaoPagamentoPedido)(int)p.SituacaoPagamento,
            SituacaoPagamentoDescricao = p.SituacaoPagamento.ToString(),
            Valor = p.Valor,
            ValorTotal = p.ValorTotal,
            QuantidadeItens = p.QuantidadeItens
        };
    }
}
