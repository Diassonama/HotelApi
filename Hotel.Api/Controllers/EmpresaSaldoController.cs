
using Hotel.Api.Controllers.Shared;
using Hotel.Application.DTOs;
using Hotel.Application.EmpresaSaldo.Commands;
using Hotel.Application.EmpresaSaldo.Queries;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmpresaSaldoController : ApiControllerBase
    {
        private readonly ILogger<EmpresaSaldoController> _logger;

        public EmpresaSaldoController(IUnitOfWork unitOfWork, ILogger<EmpresaSaldoController> logger) : base(unitOfWork)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtém o saldo de uma empresa
        /// </summary>
        /// <param name="empresaId">ID da empresa</param>
        /// <returns>Saldo da empresa</returns>
        [HttpGet("{empresaId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<EmpresaSaldoDto>> GetSaldoEmpresa(int empresaId)
        {
            try
            {
                var query = new GetSaldoEmpresaQuery { EmpresaId = empresaId };
                var result = await Mediator.Send(query);

                if (result == null)
                    return NotFound(new { message = $"Saldo não encontrado para a empresa {empresaId}" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar saldo da empresa {empresaId}");
                return BadRequest(new { message = "Erro ao buscar saldo da empresa" });
            }
        }

        /// <summary>
        /// Obtém o saldo atual de uma empresa
        /// </summary>
        /// <param name="empresaId">ID da empresa</param>
        /// <returns>Valor do saldo</returns>
        [HttpGet("{empresaId:int}/saldo-atual")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<decimal>> GetSaldoAtual(int empresaId)
        {
            try
            {
                var query = new GetSaldoAtualQuery { EmpresaId = empresaId };
                var saldo = await Mediator.Send(query);
                return Ok(new { saldo });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar saldo atual da empresa {empresaId}");
                return BadRequest(new { message = "Erro ao buscar saldo atual" });
            }
        }

        /// <summary>
        /// Obtém todos os saldos de empresas
        /// </summary>
        /// <returns>Lista de saldos</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<ActionResult<List<EmpresaSaldoDto>>> GetTodosSaldos()
        {
            try
            {
                var query = new GetTodosSaldosEmpresasQuery();
                var result = await Mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os saldos");
                return BadRequest(new { message = "Erro ao buscar saldos" });
            }
        }

        /// <summary>
        /// Processa uma movimentação de saldo (crédito ou débito)
        /// </summary>
        /// <param name="command">Comando com dados da movimentação</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("processar-movimentacao")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BaseCommandResponse>> ProcessarMovimentacao([FromBody] ProcessarMovimentacaoSaldoCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await Mediator.Send(command);

                if (result.Errors.Any())
                    return BadRequest(result);

                await _unitOfWork.Save();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar movimentação de saldo");
                return BadRequest(new { message = "Erro ao processar movimentação" });
            }
        }
        [HttpGet("empresas/{empresaId}/relatorio-movimentacoes")]
        public async Task<IActionResult> GerarRelatorioMovimentacoes(
            int empresaId,
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim,
            [FromQuery] int tipoRelatorio = 0)
        {
            var query = new GerarRelatorioMovimentacoesQuery
            {
                EmpresaId = empresaId,
                DataInicio = dataInicio,
                DataFim = dataFim,
                TipoRelatorio = (TipoRelatorio)tipoRelatorio
            };

            var pdf = await Mediator.Send(query);
            return File(pdf, "application/pdf", $"relatorio-saldo-empresa-{empresaId}.pdf");
        }

        [HttpGet("relatorio-adiantamentos-historico")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GerarRelatorioAdiantamentosHistorico(
            [FromQuery] int? empresaId,
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            try
            {
                var query = new GerarRelatorioAdiantamentosHistoricoQuery
                {
                    EmpresaId = empresaId,
                    DataInicio = dataInicio,
                    DataFim = dataFim
                };

                var pdf = await Mediator.Send(query);

                var sufixoEmpresa = empresaId.HasValue ? $"empresa-{empresaId.Value}" : "todas";
                var sufixoPeriodo = (dataInicio.HasValue || dataFim.HasValue)
                    ? $"_{dataInicio?.ToString("yyyyMMdd") ?? "inicio"}_a_{dataFim?.ToString("yyyyMMdd") ?? "hoje"}"
                    : "";

                var nomeArquivo = $"relatorio-adiantamentos-historico-{sufixoEmpresa}{sufixoPeriodo}.pdf";
                return File(pdf, "application/pdf", nomeArquivo);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Parâmetros inválidos para relatório de adiantamentos/histórico");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório de adiantamentos/histórico");
                return BadRequest(new { message = "Erro ao gerar relatório de adiantamentos/histórico" });
            }
        }
        /// <summary>
        /// Adiciona crédito ao saldo da empresa (adiantamento/depósito)
        /// </summary>
        /// <param name="command">Comando com dados do crédito</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("adicionar-credito")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BaseCommandResponse>> AdicionarCredito([FromBody] AdicionarCreditoEmpresaCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await Mediator.Send(command);

                if (result?.Errors != null && result.Errors.Any())
                    return BadRequest(result);


                await _unitOfWork.Save();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar crédito");
                return BadRequest(new { message = "Erro ao adicionar crédito" });
            }
        }

        /// <summary>
        /// Debita o saldo da empresa (uso do saldo pré-pago)
        /// </summary>
        /// <param name="command">Comando com dados do débito</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("debitar-saldo")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BaseCommandResponse>> DebitarSaldo([FromBody] DebitarSaldoEmpresaCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await Mediator.Send(command);

                /* if (result.Errors.Any())
                    return BadRequest(result); */
                if (result?.Errors != null && result.Errors.Any())
                    return BadRequest(result);

                await _unitOfWork.Save();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao debitar saldo");
                return BadRequest(new { message = "Erro ao debitar saldo" });
            }
        }

        /// <summary>
        /// Obtém o histórico de movimentações de uma empresa
        /// </summary>
        /// <param name="empresaId">ID da empresa</param>
        /// <returns>Lista de movimentações</returns>
        [HttpGet("{empresaId:int}/movimentacoes")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<EmpresaSaldoMovimentoDto>>> GetMovimentacoes(int empresaId)
        {
            try
            {
                var query = new GetMovimentacoesSaldoQuery { EmpresaId = empresaId };
                var result = await Mediator.Send(query);

                if (result == null || !result.Any())
                    return NotFound(new { message = "Nenhuma movimentação encontrada" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar movimentações da empresa {empresaId}");
                return BadRequest(new { message = "Erro ao buscar movimentações" });
            }
        }

        /// <summary>
        /// Obtém movimentações de uma empresa em um período específico
        /// </summary>
        /// <param name="empresaId">ID da empresa</param>
        /// <param name="dataInicio">Data de início</param>
        /// <param name="dataFim">Data de fim</param>
        /// <returns>Lista de movimentações no período</returns>
        [HttpGet("{empresaId:int}/movimentacoes/periodo")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<List<EmpresaSaldoMovimentoDto>>> GetMovimentacoesPeriodo(
            int empresaId,
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim)
        {
            try
            {
                var query = new GetMovimentacoesPeriodoQuery
                {
                    EmpresaId = empresaId,
                    DataInicio = dataInicio,
                    DataFim = dataFim
                };
                var result = await Mediator.Send(query);

                if (result == null || !result.Any())
                    return NotFound(new { message = "Nenhuma movimentação encontrada no período" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar movimentações do período");
                return BadRequest(new { message = "Erro ao buscar movimentações" });
            }
        }

        /// <summary>
        /// Verifica se a empresa tem saldo suficiente
        /// </summary>
        /// <param name="empresaId">ID da empresa</param>
        /// <param name="valor">Valor a verificar</param>
        /// <returns>Indica se há saldo suficiente</returns>
        [HttpGet("{empresaId:int}/verificar-saldo/{valor:decimal}")]
        [ProducesResponseType(200)]
        public async Task<ActionResult<bool>> VerificarSaldoSuficiente(int empresaId, decimal valor)
        {
            try
            {
                var query = new VerificarSaldoSuficienteQuery { EmpresaId = empresaId, Valor = valor };
                var result = await Mediator.Send(query);
                return Ok(new { temSaldoSuficiente = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao verificar saldo suficiente");
                return BadRequest(new { message = "Erro ao verificar saldo" });
            }
        }
    }
}