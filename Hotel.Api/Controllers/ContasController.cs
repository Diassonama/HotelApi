using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Contas.Commands;
using Hotel.Application.Contas.Queries;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContasController : ApiControllerBase
    {
        private readonly ILogger<ContasController> _logger;

        public ContasController(IUnitOfWork unitOfWork, ILogger<ContasController> logger) : base(unitOfWork)
        {
            _logger = logger;
        }

        /// <summary>
        /// Criar Conta a Pagar
        /// </summary>
        [HttpPost("pagar")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CriarContaPagar([FromBody] CriarContaPagarCommand command)
        {
            try
            {
                _logger.LogInformation("📝 Criando Conta a Pagar - Fornecedor: {Fornecedor}, Valor: {Valor}",
                    command.FornecedorNome, command.Valor);

                var result = await Mediator.Send(command);

                if (result.Success)
                {
                    _logger.LogInformation("✅ Conta a Pagar criada com sucesso");
                    return Ok(result);
                }

                _logger.LogWarning("⚠️ Falha ao criar Conta a Pagar: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao criar Conta a Pagar");
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "Erro interno ao criar conta a pagar",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Criar Conta a Receber de Empresa (normalmente no checkout)
        /// </summary>
        [HttpPost("receber")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CriarContaReceber([FromBody] CriarContaReceberEmpresaCheckoutCommand command)
        {
            try
            {
                _logger.LogInformation("📝 Criando Conta a Receber - Empresa: {EmpresaId}, Valor: {Valor}, Checkin: {CheckinId}",
                    command.EmpresaId, command.Valor, command.CheckinId);

                var result = await Mediator.Send(command);

                if (result.Success)
                {
                    _logger.LogInformation("✅ Conta a Receber criada com sucesso");
                    return Ok(result);
                }

                _logger.LogWarning("⚠️ Falha ao criar Conta a Receber: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao criar Conta a Receber");
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "Erro interno ao criar conta a receber",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Pagar Conta a Pagar (lança saída no caixa)
        /// </summary>
        [HttpPost("pagar/{id}/pagamento")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PagarContaPagar(int id, [FromBody] PagarContaPagarCommand command)
        {
            try
            {
                command.ContaPagarId = id;

                _logger.LogInformation("💰 Pagando Conta a Pagar #{Id} - Valor: {Valor}", id, command.Valor);

                var result = await Mediator.Send(command);

                if (result.Success)
                {
                    _logger.LogInformation("✅ Conta a Pagar #{Id} paga com sucesso. Lançado no caixa.", id);
                    return Ok(result);
                }

                _logger.LogWarning("⚠️ Falha ao pagar Conta a Pagar #{Id}: {Message}", id, result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao pagar Conta a Pagar #{Id}", id);
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "Erro interno ao pagar conta",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Receber pagamento de Conta a Receber (lança entrada no caixa)
        /// </summary>
        [HttpPost("receber/{id}/pagamento")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PagarContaReceber(int id, [FromBody] PagarContaReceberCommand command)
        {
            try
            {
                command.ContaReceberId = id;

                _logger.LogInformation("💰 Recebendo Conta a Receber #{Id} - Valor: {Valor}", id, command.Valor);

                var result = await Mediator.Send(command);

                if (result.Success)
                {
                    _logger.LogInformation("✅ Conta a Receber #{Id} recebida com sucesso. Lançado no caixa.", id);
                    return Ok(result);
                }

                _logger.LogWarning("⚠️ Falha ao receber Conta a Receber #{Id}: {Message}", id, result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao receber Conta a Receber #{Id}", id);
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "Erro interno ao receber conta",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Listar Contas a Pagar pendentes
        /// </summary>
        [HttpGet("pagar/pendentes")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ListarContasPagarPendentes()
        {
            try
            {
                var contas = await _unitOfWork.ContasPagar.GetPendentesAsync();
                return Ok(new { success = true, data = contas });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao listar contas a pagar pendentes");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Listar Contas a Receber de uma Empresa
        /// </summary>
        [HttpGet("receber/empresa/{empresaId}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ListarContasReceberEmpresa(int empresaId)
        {
            try
            {
                var contas = await _unitOfWork.ContasReceber.GetByEmpresaAsync(empresaId);
                return Ok(new { success = true, data = contas });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao listar contas a receber da empresa {EmpresaId}", empresaId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Buscar Conta a Receber de um Checkin
        /// </summary>
        [HttpGet("receber/checkin/{checkinId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> BuscarContaReceberPorCheckin(int checkinId)
        {
            try
            {
                var conta = await _unitOfWork.ContasReceber.GetByCheckinIdAsync(checkinId);

                if (conta == null)
                    return NotFound(new { success = false, message = "Conta não encontrada" });

                return Ok(new { success = true, data = conta });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao buscar conta do checkin {CheckinId}", checkinId);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // Adicionar no ContasController.cs
        [HttpGet("receber/relatorio")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GerarRelatorioContasReceber(
            [FromQuery] int? empresaId,
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            try
            {
                var query = new GerarRelatorioContasReceberQuery
                {
                    EmpresaId = empresaId,
                    DataInicio = dataInicio,
                    DataFim = dataFim
                };

                var result = await Mediator.Send(query);

                if (result.Success && result.Data is byte[] pdf)
                {
                    return File(pdf, "application/pdf", $"contas-receber-{DateTime.Now:yyyyMMdd}.pdf");
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de contas a receber");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("pagar/relatorio")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GerarRelatorioContasPagar(
            [FromQuery] int? empresaId,
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            try
            {
                var query = new GerarRelatorioContasPagarQuery
                {
                    EmpresaId = empresaId,
                    DataInicio = dataInicio,
                    DataFim = dataFim
                };

                var result = await Mediator.Send(query);

                if (result.Success && result.Data is byte[] pdf)
                {
                    return File(pdf, "application/pdf", $"contas-pagar-{DateTime.Now:yyyyMMdd}.pdf");
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de contas a pagar");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}