using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Produto.Commands.CreateProduto;
using Hotel.Application.Produto.Commands.UpdateProduto;
using Hotel.Application.Produto.Queries.GetAllProdutos;
using Hotel.Application.Produto.Queries.GetProdutoById;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProdutoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProdutoController> _logger;

        public ProdutoController(IMediator mediator, ILogger<ProdutoController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Buscar todos os produtos
        /// </summary>
        /// <param name="categoriaId">Filtro opcional por categoria</param>
        /// <param name="pontoVendasId">Filtro opcional por ponto de vendas</param>
        /// <param name="apenasAtivos">Filtro opcional apenas produtos ativos (padrão: true)</param>
        /// <param name="apenasDisponiveis">Filtro opcional apenas produtos disponíveis</param>
        /// <param name="nome">Filtro opcional por nome (busca parcial)</param>
        /// <returns>Lista de produtos</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllProdutos(
            [FromQuery] int? categoriaId = null,
            [FromQuery] int? pontoVendasId = null,
            [FromQuery] bool? apenasAtivos = true,
            [FromQuery] bool? apenasDisponiveis = null,
            [FromQuery] string nome = null)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                _logger.LogInformation("🌐 [PRODUTOS-GET-ALL-{CorrelationId}] Requisição recebida - Filtros: CategoriaId={CategoriaId}, PontoVendasId={PontoVendasId}, ApenasAtivos={ApenasAtivos}, Nome={Nome}",
                    correlationId, categoriaId, pontoVendasId, apenasAtivos, nome);

                var query = new GetAllProdutosQuery
                {
                    CategoriaId = categoriaId,
                    PontoDeVendasId = pontoVendasId,
                    ApenasAtivos = apenasAtivos,
                    ApenasDisponiveis = apenasDisponiveis,
                    Nome = nome
                };

                var result = await _mediator.Send(query);

                if (result.Success)
                {
                    _logger.LogInformation("✅ [PRODUTOS-GET-ALL-{CorrelationId}] Sucesso - {Count} produtos retornados",
                        correlationId, result.Count);

                    return Ok(new
                    {
                        success = result.Success,
                        message = result.Message,
                        data = result.Data,
                        count = result.Count,
                        correlationId
                    });
                }

                _logger.LogWarning("⚠️ [PRODUTOS-GET-ALL-{CorrelationId}] Falha: {Message}",
                    correlationId, result.Message);

                return BadRequest(new
                {
                    success = result.Success,
                    message = result.Message,
                    errors = result.Errors,
                    correlationId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PRODUTOS-GET-ALL] Erro interno: {Message}", ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro interno do servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Buscar produto por ID
        /// </summary>
        /// <param name="id">ID do produto</param>
        /// <returns>Produto encontrado</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProdutoById(int id)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                _logger.LogInformation("🌐 [PRODUTO-GET-BY-ID-{CorrelationId}] Requisição recebida - ID: {Id}",
                    correlationId, id);

                var query = new GetProdutoByIdQuery(id);
                var result = await _mediator.Send(query);

                if (result.Success)
                {
                    _logger.LogInformation("✅ [PRODUTO-GET-BY-ID-{CorrelationId}] Produto encontrado - ID: {Id}, Nome: {Nome}",
                        correlationId, result.Data?.Id, result.Data?.Nome);

                    return Ok(new
                    {
                        success = result.Success,
                        message = result.Message,
                        data = result.Data,
                        correlationId
                    });
                }

                // Produto não encontrado
                if (result.Data == null && result.Errors?.Count == 0)
                {
                    _logger.LogWarning("⚠️ [PRODUTO-GET-BY-ID-{CorrelationId}] Produto não encontrado - ID: {Id}",
                        correlationId, id);

                    return NotFound(new
                    {
                        success = result.Success,
                        message = result.Message,
                        correlationId
                    });
                }

                // Erro de validação
                _logger.LogWarning("⚠️ [PRODUTO-GET-BY-ID-{CorrelationId}] Erro de validação: {Message}",
                    correlationId, result.Message);

                return BadRequest(new
                {
                    success = result.Success,
                    message = result.Message,
                    errors = result.Errors,
                    correlationId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PRODUTO-GET-BY-ID] Erro interno ao buscar produto ID {Id}: {Message}", id, ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro interno do servidor",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Criar novo produto
        /// </summary>
        /// <param name="command">Dados do produto</param>
        /// <returns>Produto criado</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateProduto([FromBody] CreateProdutoCommand command)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                _logger.LogInformation("🌐 [PRODUTO-CREATE-{CorrelationId}] Requisição recebida - Nome: {Nome}",
                    correlationId, command?.Nome);

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    _logger.LogInformation("✅ [PRODUTO-CREATE-{CorrelationId}] Produto criado com sucesso",
                        correlationId);

                    return CreatedAtAction(
                        nameof(GetProdutoById),
                        new { id = ((Domain.Entities.Produtos)result.Data)?.Id },
                        new
                        {
                            success = result.Success,
                            message = result.Message,
                            data = result.Data,
                            correlationId
                        });
                }

                _logger.LogWarning("⚠️ [PRODUTO-CREATE-{CorrelationId}] Falha na criação: {Message}",
                    correlationId, result.Message);

                return BadRequest(new
                {
                    success = result.Success,
                    message = result.Message,
                    errors = result.Errors,
                    correlationId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PRODUTO-CREATE] Erro interno: {Message}", ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro interno do servidor",
                    error = ex.Message
                });
            }
        }
        
         /// <summary>
        /// Atualizar produto existente
        /// </summary>
        /// <param name="id">ID do produto a ser atualizado</param>
        /// <param name="command">Dados atualizados do produto</param>
        /// <returns>Produto atualizado</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateProduto(int id, [FromBody] UpdateProdutoCommand command)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];
                
                _logger.LogInformation("🌐 [PRODUTO-UPDATE-{CorrelationId}] Requisição recebida - ID: {Id}, Nome: {Nome}",
                    correlationId, id, command?.Nome);

                // ✅ GARANTIR CONSISTÊNCIA DO ID
                if (command != null)
                {
                    command.Id = id;
                }

                // ✅ VALIDAÇÃO BÁSICA
                if (id <= 0)
                {
                    _logger.LogWarning("⚠️ [PRODUTO-UPDATE-{CorrelationId}] ID inválido: {Id}",
                        correlationId, id);
                    
                    return BadRequest(new
                    {
                        success = false,
                        message = "ID do produto deve ser maior que zero",
                        correlationId
                    });
                }

                if (command == null)
                {
                    _logger.LogWarning("⚠️ [PRODUTO-UPDATE-{CorrelationId}] Command não pode ser nulo",
                        correlationId);
                    
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dados do produto são obrigatórios",
                        correlationId
                    });
                }

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    _logger.LogInformation("✅ [PRODUTO-UPDATE-{CorrelationId}] Produto atualizado com sucesso - ID: {Id}",
                        correlationId, id);
                    
                    return Ok(new
                    {
                        success = result.Success,
                        message = result.Message,
                        data = result.Data,
                        correlationId
                    });
                }

                // ✅ VERIFICAR SE É ERRO DE NÃO ENCONTRADO
                if (result.Message?.Contains("não encontrado") == true)
                {
                    _logger.LogWarning("⚠️ [PRODUTO-UPDATE-{CorrelationId}] Produto não encontrado - ID: {Id}",
                        correlationId, id);
                    
                    return NotFound(new
                    {
                        success = result.Success,
                        message = result.Message,
                        correlationId
                    });
                }

                // ✅ OUTROS ERROS DE VALIDAÇÃO
                _logger.LogWarning("⚠️ [PRODUTO-UPDATE-{CorrelationId}] Falha na atualização: {Message}",
                    correlationId, result.Message);
                
                return BadRequest(new
                {
                    success = result.Success,
                    message = result.Message,
                    errors = result.Errors,
                    correlationId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [PRODUTO-UPDATE] Erro interno ao atualizar produto ID {Id}: {Message}", id, ex.Message);
                
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro interno do servidor",
                    error = ex.Message
                });
            }
        }
    }
}