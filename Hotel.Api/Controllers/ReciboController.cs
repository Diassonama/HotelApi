using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Common;
using Hotel.Application.Reports.Commands;
using Hotel.Application.Services;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReciboController : ApiControllerBase
    {
        private readonly ILogger<ReciboController> _logger;
        private readonly UsuarioLogado _usuarioLogado;
        private readonly IEmailService _emailService;

        public ReciboController(IUnitOfWork unitOfWork, ILogger<ReciboController> logger, UsuarioLogado usuarioLogado, IEmailService emailService) : base(unitOfWork)
        {
            _logger = logger;
            _usuarioLogado = usuarioLogado;
            _emailService = emailService;
        }

        /// <summary>
        /// Gerar recibo de checkout em PDF
        /// </summary>
        /// <param name="checkinId">ID do check-in</param>
        /// <returns>PDF do recibo em base64</returns>
        [HttpGet("checkout/{checkinId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GerarReciboCheckout(int checkinId)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                _logger.LogInformation("🌐 [RECIBO-GERAR-{CorrelationId}] Requisição recebida - CheckinId: {CheckinId}",
                    correlationId, checkinId);

                var command = new GerarReciboCheckoutCommand { CheckinId = checkinId };
                var result = await Mediator.Send(command);

                if (result.Success)
                {
                    _logger.LogInformation("✅ [RECIBO-GERAR-{CorrelationId}] Recibo gerado com sucesso",
                        correlationId);

                    return Ok(new
                    {
                        success = result.Success,
                        message = result.Message,
                        data = result.Data,
                        correlationId
                    });
                }

                _logger.LogWarning("⚠️ [RECIBO-GERAR-{CorrelationId}] Falha ao gerar recibo: {Message}",
                    correlationId, result.Message);

                return BadRequest(new
                {
                    success = result.Success,
                    message = result.Message,
                    correlationId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [RECIBO-GERAR] Erro interno ao gerar recibo para CheckinId {CheckinId}: {Message}",
                    checkinId, ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro interno do servidor",
                    error = ex.Message
                });
            }
        }
        /// <summary>
        /// Download do recibo em PDF (abre/baixa o arquivo)
        /// </summary>
        /// <param name="checkinId">ID do check-in</param>
        /// <returns>Arquivo PDF</returns>
        [HttpGet("checkout/{checkinId:int}/download")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DownloadReciboCheckout(int checkinId)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                _logger.LogInformation("📥 [RECIBO-DOWNLOAD-{CorrelationId}] Requisição de download - CheckinId: {CheckinId}",
                    correlationId, checkinId);

                var command = new GerarReciboCheckoutCommand { CheckinId = checkinId };
                var result = await Mediator.Send(command);

                if (!result.Success)
                {
                    _logger.LogWarning("⚠️ [RECIBO-DOWNLOAD-{CorrelationId}] Falha ao gerar recibo: {Message}",
                        correlationId, result.Message);
                    return BadRequest(new { success = false, message = result.Message });
                }

                // Extrair PDF base64 da resposta usando reflexão
                string pdfBase64 = null;
                string nomeArquivo = $"Recibo_Checkin_{checkinId}.pdf";

                if (result.Data != null)
                {
                    var dataType = result.Data.GetType();
                    var pdfProperty = dataType.GetProperty("pdf");
                    var nomeProperty = dataType.GetProperty("nomeArquivo");

                    if (pdfProperty != null)
                    {
                        pdfBase64 = pdfProperty.GetValue(result.Data)?.ToString();
                    }

                    if (nomeProperty != null)
                    {
                        var nomeValue = nomeProperty.GetValue(result.Data)?.ToString();
                        if (!string.IsNullOrEmpty(nomeValue))
                        {
                            nomeArquivo = nomeValue;
                        }
                    }
                }

                if (string.IsNullOrEmpty(pdfBase64))
                {
                    _logger.LogError("❌ [RECIBO-DOWNLOAD-{CorrelationId}] PDF não gerado corretamente",
                        correlationId);
                    return StatusCode(500, new { success = false, message = "Erro ao processar PDF" });
                }

                // Converter base64 para bytes
                byte[] pdfBytes = Convert.FromBase64String(pdfBase64);

                _logger.LogInformation("✅ [RECIBO-DOWNLOAD-{CorrelationId}] PDF preparado para download - Tamanho: {Tamanho} bytes",
                    correlationId, pdfBytes.Length);

                // Retornar o arquivo PDF para download/abertura
                return File(pdfBytes, "application/pdf", nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [RECIBO-DOWNLOAD] Erro ao fazer download do recibo para CheckinId {CheckinId}: {Message}",
                    checkinId, ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro ao fazer download do recibo",
                    error = ex.Message
                });
            }
        }

        [HttpGet("nota-hospedagem/{checkinId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GerarNotaHospedagem(int checkinId)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                _logger.LogInformation("🌐 [NOTA-GERAR-{CorrelationId}] Requisição recebida - CheckinId: {CheckinId}",
                    correlationId, checkinId);

                var command = new GerarNotaHospedagemCommand { CheckinId = checkinId };
                var result = await Mediator.Send(command);

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = result.Success,
                        message = result.Message,
                        data = result.Data,
                        correlationId
                    });
                }

                return BadRequest(new
                {
                    success = result.Success,
                    message = result.Message,
                    correlationId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [NOTA-GERAR] Erro interno ao gerar nota para CheckinId {CheckinId}: {Message}",
                    checkinId, ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro interno do servidor",
                    error = ex.Message
                });
            }
        }

        [HttpGet("nota-hospedagem/{checkinId:int}/download")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DownloadNotaHospedagem(int checkinId)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                _logger.LogInformation("📥 [NOTA-DOWNLOAD-{CorrelationId}] Requisição de download - CheckinId: {CheckinId}",
                    correlationId, checkinId);

                var command = new GerarNotaHospedagemCommand { CheckinId = checkinId };
                var result = await Mediator.Send(command);

                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                string pdfBase64 = null;
                string nomeArquivo = $"Nota_Hospedagem_Checkin_{checkinId}.pdf";

                if (result.Data != null)
                {
                    var dataType = result.Data.GetType();
                    var pdfProperty = dataType.GetProperty("pdf");
                    var nomeProperty = dataType.GetProperty("nomeArquivo");

                    if (pdfProperty != null)
                        pdfBase64 = pdfProperty.GetValue(result.Data)?.ToString();

                    if (nomeProperty != null)
                    {
                        var nomeValue = nomeProperty.GetValue(result.Data)?.ToString();
                        if (!string.IsNullOrEmpty(nomeValue))
                            nomeArquivo = nomeValue;
                    }
                }

                if (string.IsNullOrEmpty(pdfBase64))
                    return StatusCode(500, new { success = false, message = "Erro ao processar PDF" });

                var pdfBytes = Convert.FromBase64String(pdfBase64);
                return File(pdfBytes, "application/pdf", nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [NOTA-DOWNLOAD] Erro ao fazer download da nota para CheckinId {CheckinId}: {Message}",
                    checkinId, ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro ao fazer download da nota de hospedagem",
                    error = ex.Message
                });
            }
        }

        [HttpPost("historico")]
        public async Task<IActionResult> GerarHistorico([FromBody] GerarHistoricoCommand command)
        {
            var resultado = await Mediator.Send(command);
            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpGet("historico/download")]
        /* public async Task<IActionResult> DownloadHistorico()
        {
            var command = new GerarHistoricoCommand {  Titulo = "Histórico de Ocupação" };
            var resultado = await Mediator.Send(command);

            if (!resultado.Success)
                return BadRequest(resultado);

            var pdf = (resultado.Data as dynamic)?.pdf;
            var nomeArquivo = (resultado.Data as dynamic)?.nomeArquivo;

            if (pdf == null)
                return BadRequest("PDF não gerado");

            var pdfBytes = Convert.FromBase64String(pdf);
            return File(pdfBytes, "application/pdf", nomeArquivo?.ToString() ?? "Historico_Ocupacao.pdf");
        } */

        public async Task<IActionResult> DownloadHistorico()
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                _logger.LogInformation("📥 [HISTORICO-DOWNLOAD-{CorrelationId}] Requisição de download ",
                    correlationId);

                var command = new GerarHistoricoCommand { Titulo = "Histórico de Ocupação" };
                var resultado = await Mediator.Send(command);

                if (!resultado.Success)
                {
                    _logger.LogWarning("⚠️ [HISTORICO-DOWNLOAD-{CorrelationId}] Falha ao gerar histórico: {Message}",
                        correlationId, resultado.Message);
                    return BadRequest(resultado);
                }

                // Extrair PDF base64 da resposta
                string pdfBase64 = null;
                string nomeArquivo = $"Historico_Ocupacao_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

                if (resultado.Data != null)
                {
                    var dataType = resultado.Data.GetType();
                    var pdfProperty = dataType.GetProperty("pdf");
                    var nomeProperty = dataType.GetProperty("nomeArquivo");

                    if (pdfProperty != null)
                    {
                        pdfBase64 = pdfProperty.GetValue(resultado.Data)?.ToString();
                    }

                    if (nomeProperty != null)
                    {
                        var nomeValue = nomeProperty.GetValue(resultado.Data)?.ToString();
                        if (!string.IsNullOrEmpty(nomeValue))
                        {
                            nomeArquivo = nomeValue;
                        }
                    }
                }

                if (string.IsNullOrEmpty(pdfBase64))
                {
                    _logger.LogError("❌ [HISTORICO-DOWNLOAD-{CorrelationId}] PDF não gerado corretamente",
                        correlationId);
                    return StatusCode(500, new { success = false, message = "Erro ao processar PDF" });
                }

                // Converter base64 para bytes
                byte[] pdfBytes = Convert.FromBase64String(pdfBase64);

                _logger.LogInformation("✅ [HISTORICO-DOWNLOAD-{CorrelationId}] PDF preparado para download - Tamanho: {Tamanho} bytes",
                    correlationId, pdfBytes.Length);

                // Retornar o arquivo PDF para download/abertura
                return File(pdfBytes, "application/pdf", nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [HISTORICO-DOWNLOAD] Erro ao fazer download do histórico para EmpresaId  {Message}"
                    , ex.Message);

                return StatusCode(500, new { success = false, message = "Erro ao fazer download do histórico", error = ex.Message });
            }
        }

        [HttpPost("governanca-arrumacao")]
        public async Task<IActionResult> GerarGovernancaArrumacao([FromBody] GerarGovernancaArrumacaoCommand command)
        {
            var resultado = await Mediator.Send(command);
            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpGet("governanca-arrumacao/download")]
        public async Task<IActionResult> DownloadGovernancaArrumacao()
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                _logger.LogInformation("📥 [GOV-ARRUMACAO-DOWNLOAD-{CorrelationId}] Requisição de download", correlationId);

                var command = new GerarGovernancaArrumacaoCommand { Titulo = "GOVERNANÇA RELATÓRIO DE ARRUMAÇÃO" };
                var resultado = await Mediator.Send(command);

                if (!resultado.Success)
                    return BadRequest(resultado);

                string pdfBase64 = null;
                string nomeArquivo = $"Governanca_Arrumacao_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

                if (resultado.Data != null)
                {
                    var dataType = resultado.Data.GetType();
                    var pdfProperty = dataType.GetProperty("pdf");
                    var nomeProperty = dataType.GetProperty("nomeArquivo");

                    if (pdfProperty != null)
                        pdfBase64 = pdfProperty.GetValue(resultado.Data)?.ToString();

                    if (nomeProperty != null)
                    {
                        var nomeValue = nomeProperty.GetValue(resultado.Data)?.ToString();
                        if (!string.IsNullOrEmpty(nomeValue))
                            nomeArquivo = nomeValue;
                    }
                }

                if (string.IsNullOrEmpty(pdfBase64))
                    return StatusCode(500, new { success = false, message = "Erro ao processar PDF" });

                var pdfBytes = Convert.FromBase64String(pdfBase64);
                return File(pdfBytes, "application/pdf", nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GOV-ARRUMACAO-DOWNLOAD] Erro ao fazer download: {Message}", ex.Message);
                return StatusCode(500, new { success = false, message = "Erro ao fazer download do relatório de governança", error = ex.Message });
            }
        }

        [HttpPost("vistoria-quartos")]
        public async Task<IActionResult> GerarVistoriaQuartos([FromBody] GerarVistoriaQuartosCommand command)
        {
            var resultado = await Mediator.Send(command);
            if (!resultado.Success)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpGet("vistoria-quartos/download")]
        public async Task<IActionResult> DownloadVistoriaQuartos()
        {
            try
            {
                var command = new GerarVistoriaQuartosCommand { Titulo = "Vistória de Quartos" };
                var resultado = await Mediator.Send(command);

                if (!resultado.Success)
                    return BadRequest(resultado);

                string pdfBase64 = null;
                string nomeArquivo = $"Vistoria_Quartos_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

                if (resultado.Data != null)
                {
                    var dataType = resultado.Data.GetType();
                    var pdfProperty = dataType.GetProperty("pdf");
                    var nomeProperty = dataType.GetProperty("nomeArquivo");

                    if (pdfProperty != null)
                        pdfBase64 = pdfProperty.GetValue(resultado.Data)?.ToString();

                    if (nomeProperty != null)
                    {
                        var nomeValue = nomeProperty.GetValue(resultado.Data)?.ToString();
                        if (!string.IsNullOrEmpty(nomeValue))
                            nomeArquivo = nomeValue;
                    }
                }

                if (string.IsNullOrEmpty(pdfBase64))
                    return StatusCode(500, new { success = false, message = "Erro ao processar PDF" });

                var pdfBytes = Convert.FromBase64String(pdfBase64);
                return File(pdfBytes, "application/pdf", nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [VISTORIA-QUARTOS-DOWNLOAD] Erro ao fazer download: {Message}", ex.Message);
                return StatusCode(500, new { success = false, message = "Erro ao fazer download do relatório de vistoria", error = ex.Message });
            }
        }

        /// <summary>
        /// Gerar movimento diário em PDF (JSON com base64)
        /// </summary>
        [HttpGet("movimento-diario")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GerarMovimentoDiario([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];
                var dataInicioFiltro = dataInicio?.Date ?? DateTime.Today;
                var dataFimFiltro = dataFim?.Date ?? DateTime.Today;

                if (dataFimFiltro < dataInicioFiltro)
                    return BadRequest(new { success = false, message = "A dataFim não pode ser menor que a dataInicio.", correlationId });

                _logger.LogInformation("🌐 [MOV-DIARIO-{CorrelationId}] Requisição recebida - Período: {DataInicio} a {DataFim}",
                    correlationId, dataInicioFiltro.ToString("dd/MM/yyyy"), dataFimFiltro.ToString("dd/MM/yyyy"));

                var command = new GerarMovimentoDiarioCommand { DataInicio = dataInicioFiltro, DataFim = dataFimFiltro };
                var result = await Mediator.Send(command);

                if (result.Success)
                    return Ok(new { success = true, message = result.Message, data = result.Data, correlationId });

                return BadRequest(new { success = false, message = result.Message, correlationId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [MOV-DIARIO] Erro ao gerar movimento diário: {Message}", ex.Message);
                return StatusCode(500, new { success = false, message = "Erro interno do servidor", error = ex.Message });
            }
        }

        /// <summary>
        /// Download do movimento diário em PDF
        /// </summary>
        [HttpGet("movimento-diario/download")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DownloadMovimentoDiario([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];
                var dataInicioFiltro = dataInicio?.Date ?? DateTime.Today;
                var dataFimFiltro = dataFim?.Date ?? DateTime.Today;

                if (dataFimFiltro < dataInicioFiltro)
                    return BadRequest(new { success = false, message = "A dataFim não pode ser menor que a dataInicio." });

                _logger.LogInformation("📥 [MOV-DIARIO-DL-{CorrelationId}] Download - Período: {DataInicio} a {DataFim}",
                    correlationId, dataInicioFiltro.ToString("dd/MM/yyyy"), dataFimFiltro.ToString("dd/MM/yyyy"));

                var command = new GerarMovimentoDiarioCommand { DataInicio = dataInicioFiltro, DataFim = dataFimFiltro };
                var result = await Mediator.Send(command);

                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                string pdfBase64 = null;
                string nomeArquivo = $"Movimento_Diario_{dataInicioFiltro:yyyyMMdd}_a_{dataFimFiltro:yyyyMMdd}.pdf";

                if (result.Data != null)
                {
                    var dataType = result.Data.GetType();
                    pdfBase64 = dataType.GetProperty("pdf")?.GetValue(result.Data)?.ToString();
                    var nome = dataType.GetProperty("nomeArquivo")?.GetValue(result.Data)?.ToString();
                    if (!string.IsNullOrEmpty(nome)) nomeArquivo = nome;
                }

                if (string.IsNullOrEmpty(pdfBase64))
                    return StatusCode(500, new { success = false, message = "Erro ao processar PDF" });

                var pdfBytes = Convert.FromBase64String(pdfBase64);
                return File(pdfBytes, "application/pdf", nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [MOV-DIARIO-DL] Erro ao fazer download: {Message}", ex.Message);
                return StatusCode(500, new { success = false, message = "Erro ao fazer download do movimento diário", error = ex.Message });
            }
        }

        /// <summary>
        /// Gerar lançamento do caixa do dia em PDF (JSON com base64)
        /// </summary>
        [HttpGet("movimento-caixa")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GerarMovimentoCaixa(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];
                var dataInicioFiltro = dataInicio ?? DateTime.Today;
                var dataFimFiltro = dataFim ?? DateTime.Today;
                var perfil = _usuarioLogado.Role;
                var usuario = _usuarioLogado.UserId;

                _logger.LogInformation("💰 [MOV-CAIXA-{CorrelationId}] Requisição recebida - Data: {DataInicio} a {DataFim} - Usuário: {Usuario} ({Perfil})",
                    correlationId, dataInicioFiltro.ToString("dd/MM/yyyy"), dataFimFiltro.ToString("dd/MM/yyyy"), usuario, perfil);

                var command = new GerarMovimentoCaixaCommand
                {
                    DataInicio = dataInicioFiltro,
                    DataFim = dataFimFiltro,
                    Perfil = perfil,
                    Usuario = usuario
                };
                var result = await Mediator.Send(command);

                if (result.Success)
                {
                    _logger.LogInformation("✅ [MOV-CAIXA-{CorrelationId}] Movimento de caixa gerado com sucesso",
                        correlationId);

                    return Ok(new
                    {
                        success = result.Success,
                        message = result.Message,
                        data = result.Data,
                        correlationId
                    });
                }

                _logger.LogWarning("⚠️ [MOV-CAIXA-{CorrelationId}] Falha ao gerar movimento: {Message}",
                    correlationId, result.Message);

                return BadRequest(new
                {
                    success = result.Success,
                    message = result.Message,
                    correlationId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [MOV-CAIXA] Erro ao gerar movimento de caixa: {Message}", ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro ao gerar movimento de caixa",
                    error = ex.Message,
                    correlationId = Guid.NewGuid().ToString("N")[..8]
                });
            }
        }

        /// <summary>
        /// Download do PDF de movimento de caixa
        /// </summary>
        [HttpGet("movimento-caixa/download")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DownloadMovimentoCaixa(
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];
                var dataInicioFiltro = dataInicio ?? DateTime.Today;
                var dataFimFiltro = dataFim ?? DateTime.Today;
                var perfil = _usuarioLogado.Role;
                var usuario = _usuarioLogado.UserId;

                _logger.LogInformation("📥 [MOV-CAIXA-DOWNLOAD-{CorrelationId}] Requisição de download - Data: {DataInicio} a {DataFim} - Usuário: {Usuario} ({Perfil})",
                    correlationId, dataInicioFiltro.ToString("dd/MM/yyyy"), dataFimFiltro.ToString("dd/MM/yyyy"), usuario, perfil);

                var command = new GerarMovimentoCaixaCommand
                {
                    DataInicio = dataInicioFiltro,
                    DataFim = dataFimFiltro,
                    Perfil = perfil,
                    Usuario = usuario
                };
                var result = await Mediator.Send(command);

                if (!result.Success)
                {
                    _logger.LogWarning("⚠️ [MOV-CAIXA-DOWNLOAD-{CorrelationId}] Falha ao gerar: {Message}",
                        correlationId, result.Message);
                    return BadRequest(result);
                }

                string pdfBase64 = null;
                string nomeArquivo = $"Movimento_Caixa_{dataInicioFiltro:yyyyMMdd}_{dataFimFiltro:yyyyMMdd}.pdf";

                if (result.Data != null)
                {
                    var dataType = result.Data.GetType();
                    var pdfProperty = dataType.GetProperty("pdf");
                    var nomeProperty = dataType.GetProperty("nomeArquivo");

                    if (pdfProperty != null)
                        pdfBase64 = pdfProperty.GetValue(result.Data)?.ToString();

                    if (nomeProperty != null)
                    {
                        var nomeValue = nomeProperty.GetValue(result.Data)?.ToString();
                        if (!string.IsNullOrEmpty(nomeValue))
                            nomeArquivo = nomeValue;
                    }
                }

                if (string.IsNullOrEmpty(pdfBase64))
                {
                    _logger.LogError("❌ [MOV-CAIXA-DOWNLOAD-{CorrelationId}] PDF não gerado corretamente", correlationId);
                    return StatusCode(500, new { success = false, message = "Erro ao processar PDF" });
                }

                byte[] pdfBytes = Convert.FromBase64String(pdfBase64);

                _logger.LogInformation("✅ [MOV-CAIXA-DOWNLOAD-{CorrelationId}] PDF preparado - Tamanho: {Tamanho} bytes",
                    correlationId, pdfBytes.Length);

                return File(pdfBytes, "application/pdf", nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [MOV-CAIXA-DOWNLOAD] Erro: {Message}", ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro ao fazer download do movimento de caixa",
                    error = ex.Message
                });
            }
        }

        [HttpPost("checkout/{checkinId:int}/enviar-email")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EnviarDocumentoPorEmail(int checkinId, [FromBody] EnviarDocumentoEmailRequest request)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];
                var tipoDocumento = (request?.TipoDocumento ?? "recibo").Trim().ToLowerInvariant();

                if (tipoDocumento != "recibo" && tipoDocumento != "factura" && tipoDocumento != "fatura")
                    return BadRequest(new { success = false, message = "tipoDocumento deve ser 'recibo' ou 'factura'.", correlationId });

                var gerarResultado = (tipoDocumento == "recibo")
                    ? await Mediator.Send(new GerarReciboCheckoutCommand { CheckinId = checkinId })
                    : await Mediator.Send(new GerarNotaHospedagemCommand { CheckinId = checkinId });

                if (!gerarResultado.Success)
                    return BadRequest(new { success = false, message = gerarResultado.Message, correlationId });

                if (!TryExtractPdf(gerarResultado.Data, out var pdfBase64, out var nomeArquivo) || string.IsNullOrWhiteSpace(pdfBase64))
                    return StatusCode(500, new { success = false, message = "Erro ao processar PDF do documento.", correlationId });

                var emailDestino = request?.EmailDestino;
                if (string.IsNullOrWhiteSpace(emailDestino))
                    emailDestino = await ObterEmailDestinoPorCheckin(checkinId, request?.EnviarParaEmpresa ?? false);

                if (string.IsNullOrWhiteSpace(emailDestino))
                    return NotFound(new { success = false, message = "Não foi possível determinar o email do cliente/empresa.", correlationId });

                var assunto = tipoDocumento == "recibo"
                    ? $"GHOTEL - Recibo do check-in #{checkinId}"
                    : $"GHOTEL - Factura do check-in #{checkinId}";

                var corpo = tipoDocumento == "recibo"
                    ? $"<p>Olá,</p><p>Segue em anexo o recibo referente ao check-in <strong>#{checkinId}</strong>.</p><p>Atenciosamente,<br/>Equipe GHOTEL</p>"
                    : $"<p>Olá,</p><p>Segue em anexo a factura referente ao check-in <strong>#{checkinId}</strong>.</p><p>Atenciosamente,<br/>Equipe GHOTEL</p>";

                await _emailService.SendEmailWithAttachment(
                    emailDestino,
                    assunto,
                    corpo,
                    Convert.FromBase64String(pdfBase64),
                    string.IsNullOrWhiteSpace(nomeArquivo)
                        ? (tipoDocumento == "recibo" ? $"Recibo_Checkin_{checkinId}.pdf" : $"Factura_Checkin_{checkinId}.pdf")
                        : nomeArquivo,
                    "application/pdf");

                _logger.LogInformation("📧 [DOC-EMAIL-{CorrelationId}] Documento enviado por email para {Email}. CheckinId: {CheckinId}, Tipo: {Tipo}",
                    correlationId, emailDestino, checkinId, tipoDocumento);

                return Ok(new
                {
                    success = true,
                    message = "Documento enviado por email com sucesso.",
                    data = new
                    {
                        checkinId,
                        tipoDocumento,
                        emailDestino,
                        nomeArquivo
                    },
                    correlationId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [DOC-EMAIL] Erro ao enviar documento por email para CheckinId {CheckinId}: {Message}",
                    checkinId, ex.Message);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro interno do servidor ao enviar documento por email.",
                    error = ex.Message
                });
            }
        }

        private static bool TryExtractPdf(object data, out string pdfBase64, out string nomeArquivo)
        {
            pdfBase64 = null;
            nomeArquivo = null;

            if (data == null)
                return false;

            var dataType = data.GetType();
            pdfBase64 = dataType.GetProperty("pdf")?.GetValue(data)?.ToString();
            nomeArquivo = dataType.GetProperty("nomeArquivo")?.GetValue(data)?.ToString();

            return !string.IsNullOrWhiteSpace(pdfBase64);
        }

        #region Recibo Pedido

        /// <summary>
        /// Gerar recibo de pedido (restaurante/frigobar) em PDF base64
        /// </summary>
        [HttpGet("pedido/{pedidoId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GerarReciboPedido(int pedidoId)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("📄 [RECIBO-PEDIDO-{CorrelationId}] GET pedido/{PedidoId}", correlationId, pedidoId);

                var command = new GerarReciboPedidoCommand { PedidoId = pedidoId };
                var result = await Mediator.Send(command);

                if (!result.Success)
                {
                    _logger.LogWarning("⚠️ [RECIBO-PEDIDO-{CorrelationId}] Falha: {Message}", correlationId, result.Message);
                    return BadRequest(new { success = false, message = result.Message });
                }

                _logger.LogInformation("✅ [RECIBO-PEDIDO-{CorrelationId}] Recibo gerado para PedidoId {PedidoId}", correlationId, pedidoId);
                return Ok(new { success = true, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [RECIBO-PEDIDO-{CorrelationId}] Erro PedidoId {PedidoId}: {Message}", correlationId, pedidoId, ex.Message);
                return StatusCode(500, new { success = false, message = "Erro ao gerar recibo do pedido", error = ex.Message });
            }
        }

        /// <summary>
        /// Download direto do recibo de pedido em PDF
        /// </summary>
        [HttpGet("pedido/{pedidoId:int}/download")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DownloadReciboPedido(int pedidoId)
        {
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("📥 [RECIBO-PEDIDO-DL-{CorrelationId}] Download PedidoId {PedidoId}", correlationId, pedidoId);

                var command = new GerarReciboPedidoCommand { PedidoId = pedidoId };
                var result = await Mediator.Send(command);

                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                string nomeArquivo = $"Recibo_Pedido_{pedidoId}.pdf";

                if (!TryExtractPdf(result.Data, out var pdfBase64, out var downloadNome))
                    return StatusCode(500, new { success = false, message = "Erro ao processar PDF" });

                if (!string.IsNullOrEmpty(downloadNome))
                    nomeArquivo = downloadNome;

                byte[] pdfBytes = Convert.FromBase64String(pdfBase64);

                _logger.LogInformation("✅ [RECIBO-PEDIDO-DL-{CorrelationId}] Download pronto — {Bytes} bytes", correlationId, pdfBytes.Length);
                return File(pdfBytes, "application/pdf", nomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [RECIBO-PEDIDO-DL-{CorrelationId}] Erro PedidoId {PedidoId}: {Message}", correlationId, pedidoId, ex.Message);
                return StatusCode(500, new { success = false, message = "Erro ao fazer download do recibo", error = ex.Message });
            }
        }

        #endregion

        private async Task<string> ObterEmailDestinoPorCheckin(int checkinId, bool enviarParaEmpresa)
        {
            var hospede = await _unitOfWork.hospedes.GetByCheckinIdAsync(checkinId);
            if (hospede == null)
                return null;

            var emailCliente = hospede.Clientes?.Email;

            if (!enviarParaEmpresa && hospede.Estado != Hotel.Domain.Entities.Hospede.EstadoHospede.Empresa)
                return emailCliente;

            var empresaId = hospede.Clientes?.EmpresasId ?? 0;
            if (empresaId <= 0)
                return emailCliente;

            var empresa = await _unitOfWork.Empresa.Get(empresaId);
            return string.IsNullOrWhiteSpace(empresa?.Email) ? emailCliente : empresa.Email;
        }

        public class EnviarDocumentoEmailRequest
        {
            public string TipoDocumento { get; set; } = "recibo";
            public string EmailDestino { get; set; }
            public bool EnviarParaEmpresa { get; set; }
        }
    }
}