using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Pagamento;
using Hotel.Application.Pagamento.Queries.GetPagamentoById;
using Hotel.Application.Pagamento.Queries.GetPagamentosByCheckinId;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PagamentoController : ApiControllerBase
    {

        private readonly ILogger<PagamentoController> _logger;

        public PagamentoController(IUnitOfWork unitOfWork, ILogger<PagamentoController> logger) : base(unitOfWork)
        {
            _logger = logger;
        }
        /*  public PagamentoController(IUnitOfWork unitOfWork) : base(unitOfWork)
         {
         } */
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePagamentoCommand createPagamentoCommand)
        {
            var resposta = await Mediator.Send(createPagamentoCommand, CancellationToken.None);
            return Ok(resposta);     //await ResponseAsync((BaseCommandResponse)resposta);
        }
        [HttpGet("AnularPagamento")]
        public async Task<BaseCommandResponse> Get(int id)
        {
            return await Mediator.Send(new AnularPagamentoCommand { PagamentoId = id });
        }
        /// <summary>
        /// Buscar pagamento por ID
        /// </summary>
        /// <param name="id">ID do pagamento</param>
        /// <returns>Pagamento encontrado</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetByIdPagamento(int id)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                _logger.LogInformation("🌐 [PAGAMENTO-GET-BY-ID-{CorrelationId}] Requisição recebida - ID: {Id}",
                    correlationId, id);

                var query = new GetPagamentoByIdQuery(id);
                var result = await Mediator.Send(query);

                if (result.Success)
                {
                    _logger.LogInformation("✅ [PAGAMENTO-GET-BY-ID-{CorrelationId}] Pagamento encontrado - ID: {Id}, Valor: {Valor}",
                        correlationId, result.Data?.Id, result.Data?.Valor);

                    return Ok(new
                    {
                        success = result.Success,
                        message = result.Message,
                        data = result.Data,
                        correlationId
                    });
                }

                // Pagamento não encontrado
                if (result.Data == null && (result.Errors == null || result.Errors.Count == 0))
                {
                    _logger.LogWarning("⚠️ [PAGAMENTO-GET-BY-ID-{CorrelationId}] Pagamento não encontrado - ID: {Id}",
                        correlationId, id);

                    return NotFound(new
                    {
                        success = result.Success,
                        message = result.Message,
                        correlationId
                    });
                }

                // Erro de validação
                _logger.LogWarning("⚠️ [PAGAMENTO-GET-BY-ID-{CorrelationId}] Erro de validação: {Message}",
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
                _logger.LogError(ex, "❌ [PAGAMENTO-GET-BY-ID] Erro interno ao buscar pagamento ID {Id}: {Message}", id, ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro interno do servidor",
                    error = ex.Message
                });
            }
        }

          /// <summary>
        /// Buscar pagamentos por CheckinId
        /// </summary>
        /// <param name="checkinId">ID do check-in</param>
        /// <returns>Lista de pagamentos do check-in</returns>
        [HttpGet("checkin/{checkinId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetByCheckinId(int checkinId)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];
                
                _logger.LogInformation("🌐 [PAGAMENTOS-GET-BY-CHECKIN-{CorrelationId}] Requisição recebida - CheckinId: {CheckinId}",
                    correlationId, checkinId);

                var query = new GetPagamentosByCheckinIdQuery(checkinId);
                var result = await Mediator.Send(query);

                

                 if (result.Success)
                {
                    /*   var valorTotal = (result.Data as IEnumerable<Pagamento>)?.Sum(p => p.Valor) ?? 0;
                    var totalPagamentos = result.Data?.Count ?? 0;
                   */
                    _logger.LogInformation("✅ [PAGAMENTOS-GET-BY-CHECKIN-{CorrelationId}] Busca concluída - CheckinId: {CheckinId},  ValorTotal: {ValorTotal}",
                        correlationId, checkinId, (result.Data as IEnumerable<Pagamento>)?.Sum(p => p.Valor) ?? 0);
                    
                      return Ok(new
                    {
                        success = result.Success,
                        message = result.Message,
                        data = result.Data,
                       
                     /*    summary = new
                        {
                            checkinId = checkinId,
                           
                            valorTotal = (result.Data as IEnumerable<Pagamento>)?.Sum(p => p.Valor) ?? 0
                           
                        }, */
                        correlationId
                    });  
                    
                }
 
                // Check-in não encontrado ou erro de validação
                if (result.Message?.Contains("não encontrado") == true)
                {
                    _logger.LogWarning("⚠️ [PAGAMENTOS-GET-BY-CHECKIN-{CorrelationId}] Check-in não encontrado - CheckinId: {CheckinId}",
                        correlationId, checkinId);
                    
                    return NotFound(new
                    {
                        success = result.Success,
                        message = result.Message,
                        correlationId
                    });
                }

                // Outros erros de validação
                _logger.LogWarning("⚠️ [PAGAMENTOS-GET-BY-CHECKIN-{CorrelationId}] Erro de validação: {Message}",
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
                _logger.LogError(ex, "❌ [PAGAMENTOS-GET-BY-CHECKIN] Erro interno ao buscar pagamentos do CheckinId {CheckinId}: {Message}", checkinId, ex.Message);
                
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