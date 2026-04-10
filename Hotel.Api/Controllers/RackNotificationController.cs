using Hotel.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RackNotificationController : ControllerBase
    {
        private readonly IRackNotificationService _rackNotificationService;
        private readonly ILogger<RackNotificationController> _logger;

        public RackNotificationController(
            IRackNotificationService rackNotificationService,
            ILogger<RackNotificationController> logger)
        {
            _rackNotificationService = rackNotificationService;
            _logger = logger;
        }

        /// <summary>
        /// Teste de notificação geral do rack
        /// </summary>
        [HttpPost("test-notification")]
        public async Task<IActionResult> TestNotification()
        {
            try
            {
                await _rackNotificationService.NotifyRackUpdateAsync();
                return Ok(new { message = "Notificação de teste enviada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar notificação de teste");
                return BadRequest($"Erro ao enviar notificação: {ex.Message}");
            }
        }

        /// <summary>
        /// Teste de notificação de informação
        /// </summary>
        [HttpPost("test-info")]
        public async Task<IActionResult> TestInfoNotification([FromBody] string message)
        {
            try
            {
                await _rackNotificationService.NotifyInfoAsync(message, new { Source = "API Test" });
                return Ok(new { message = "Notificação de informação enviada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar notificação de informação");
                return BadRequest($"Erro ao enviar notificação: {ex.Message}");
            }
        }

        /// <summary>
        /// Teste de notificação de erro
        /// </summary>
        [HttpPost("test-error")]
        public async Task<IActionResult> TestErrorNotification([FromBody] string message)
        {
            try
            {
                await _rackNotificationService.NotifyErrorAsync(message, "Teste de notificação de erro via API");
                return Ok(new { message = "Notificação de erro enviada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar notificação de erro");
                return BadRequest($"Erro ao enviar notificação: {ex.Message}");
            }
        }

        /// <summary>
        /// Força uma atualização das métricas do dashboard
        /// </summary>
        [HttpPost("update-dashboard-metrics")]
        public async Task<IActionResult> UpdateDashboardMetrics()
        {
            try
            {
                await _rackNotificationService.NotifyInfoAsync("Dashboard metrics update requested", 
                    new { RequestedAt = DateTime.Now, Source = "Manual API Call" });
                
                return Ok(new { message = "Solicitação de atualização das métricas enviada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao solicitar atualização das métricas");
                return BadRequest($"Erro ao solicitar atualização: {ex.Message}");
            }
        }

        /// <summary>
        /// Envia notificação via WhatsApp Business para hóspedes
        /// </summary>
        [HttpPost("whatsapp/send-notification")]
        public async Task<IActionResult> SendWhatsAppNotification([FromBody] WhatsAppNotificationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Log da tentativa de envio
                _logger.LogInformation("Tentativa de envio de WhatsApp para {PhoneNumber}, Tipo: {MessageType}", 
                    request.PhoneNumber, request.MessageType);

                // Preparar dados da notificação
                var notificationData = new
                {
                    PhoneNumber = request.PhoneNumber,
                    MessageType = request.MessageType,
                    Content = request.Content,
                    HospedeId = request.HospedeId,
                    CheckinId = request.CheckinId,
                    ApartamentoCodigo = request.ApartamentoCodigo,
                    Timestamp = DateTime.Now,
                    Source = "WhatsApp Business API"
                };

                // Validar número de telefone
                if (!IsValidPhoneNumber(request.PhoneNumber))
                {
                    return BadRequest(new { message = "Número de telefone inválido. Use formato internacional (+351xxxxxxxxx)" });
                }

                // Criar mensagem baseada no tipo
                string message = CreateWhatsAppMessage(request);

                // Enviar notificação via RackHub
                await _rackNotificationService.NotifyInfoAsync(
                    $"WhatsApp {request.MessageType} enviado para {request.PhoneNumber}", 
                    notificationData);

                // Simular envio para WhatsApp Business API
                // Aqui você integraria com a API real do WhatsApp Business
                await SimulateWhatsAppSend(request, message);

                return Ok(new 
                { 
                    message = "Notificação WhatsApp enviada com sucesso",
                    phoneNumber = request.PhoneNumber,
                    messageType = request.MessageType,
                    sentAt = DateTime.Now,
                    messageContent = message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar notificação WhatsApp para {PhoneNumber}", request?.PhoneNumber);
                return BadRequest($"Erro ao enviar WhatsApp: {ex.Message}");
            }
        }

        /// <summary>
        /// Webhook para receber status de entrega do WhatsApp Business
        /// </summary>
        [HttpPost("whatsapp/webhook")]
        [AllowAnonymous] // Webhook público do WhatsApp
        public async Task<IActionResult> WhatsAppWebhook([FromBody] WhatsAppWebhookRequest webhook)
        {
            try
            {
                _logger.LogInformation("Webhook WhatsApp recebido: {MessageId}, Status: {Status}", 
                    webhook.MessageId, webhook.Status);

                // Processar status de entrega
                var notificationData = new
                {
                    MessageId = webhook.MessageId,
                    Status = webhook.Status,
                    PhoneNumber = webhook.PhoneNumber,
                    Timestamp = webhook.Timestamp,
                    Error = webhook.Error,
                    Source = "WhatsApp Webhook"
                };

                // Notificar status via RackHub
                string statusMessage = webhook.Status.ToLower() switch
                {
                    "delivered" => $"✅ WhatsApp entregue para {webhook.PhoneNumber}",
                    "read" => $"👁️ WhatsApp lido por {webhook.PhoneNumber}",
                    "failed" => $"❌ Falha no envio WhatsApp para {webhook.PhoneNumber}: {webhook.Error}",
                    "sent" => $"📤 WhatsApp enviado para {webhook.PhoneNumber}",
                    _ => $"ℹ️ Status WhatsApp: {webhook.Status} para {webhook.PhoneNumber}"
                };

                await _rackNotificationService.NotifyInfoAsync(statusMessage, notificationData);

                return Ok(new { message = "Webhook processado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar webhook WhatsApp");
                return BadRequest($"Erro ao processar webhook: {ex.Message}");
            }
        }

        /// <summary>
        /// Lista templates de mensagens WhatsApp disponíveis
        /// </summary>
        [HttpGet("whatsapp/templates")]
        public IActionResult GetWhatsAppTemplates()
        {
            try
            {
                var templates = new
                {
                    checkin_confirmation = new
                    {
                        name = "checkin_confirmation",
                        description = "Confirmação de check-in",
                        template = "Olá {{nome}}! Bem-vindo ao nosso hotel. Seu check-in no apartamento {{apartamento}} foi confirmado. Qualquer dúvida, estamos à disposição!"
                    },
                    checkout_reminder = new
                    {
                        name = "checkout_reminder",
                        description = "Lembrete de check-out",
                        template = "Olá {{nome}}! Lembramos que seu check-out está previsto para hoje às {{hora}}. Apartamento: {{apartamento}}. Obrigado pela estadia!"
                    },
                    payment_confirmation = new
                    {
                        name = "payment_confirmation",
                        description = "Confirmação de pagamento",
                        template = "Pagamento de {{valor}} confirmado! Obrigado {{nome}}. Recibo disponível na recepção."
                    },
                    welcome_message = new
                    {
                        name = "welcome_message",
                        description = "Mensagem de boas-vindas",
                        template = "🏨 Bem-vindo ao nosso hotel, {{nome}}! Esperamos que tenha uma estadia incrível. WiFi: {{wifi_password}}"
                    },
                    custom_message = new
                    {
                        name = "custom_message",
                        description = "Mensagem personalizada",
                        template = "Texto livre definido pelo usuário"
                    }
                };

                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter templates WhatsApp");
                return BadRequest($"Erro ao obter templates: {ex.Message}");
            }
        }

        #region Private Methods

        /// <summary>
        /// Valida formato do número de telefone
        /// </summary>
        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Formato internacional: +[código país][número]
            return phoneNumber.StartsWith("+") && 
                   phoneNumber.Length >= 10 && 
                   phoneNumber.Length <= 15 &&
                   phoneNumber.Substring(1).All(char.IsDigit);
        }

        /// <summary>
        /// Cria mensagem formatada baseada no tipo
        /// </summary>
        private static string CreateWhatsAppMessage(WhatsAppNotificationRequest request)
        {
            return request.MessageType.ToLower() switch
            {
                "checkin_confirmation" => $"🏨 Olá! Bem-vindo ao nosso hotel. Seu check-in no apartamento {request.ApartamentoCodigo} foi confirmado. {request.Content}",
                "checkout_reminder" => $"📅 Lembrete: Seu check-out está previsto para hoje. Apartamento: {request.ApartamentoCodigo}. {request.Content}",
                "payment_confirmation" => $"💰 Pagamento confirmado! Obrigado. {request.Content}",
                "welcome_message" => $"🎉 Bem-vindo! Esperamos que tenha uma estadia incrível. {request.Content}",
                "custom_message" => request.Content,
                _ => $"📢 Notificação do hotel: {request.Content}"
            };
        }

        /// <summary>
        /// Simula envio para WhatsApp Business API
        /// Em produção, substitua por integração real
        /// </summary>
        private async Task SimulateWhatsAppSend(WhatsAppNotificationRequest request, string message)
        {
            // Simular delay de envio
            await Task.Delay(500);

            // Log do envio simulado
            _logger.LogInformation("📱 WhatsApp SIMULADO enviado para {PhoneNumber}: {Message}", 
                request.PhoneNumber, message);

            // Aqui você faria a integração real com WhatsApp Business API:
            // 
            // using var httpClient = new HttpClient();
            // var whatsappPayload = new
            // {
            //     messaging_product = "whatsapp",
            //     to = request.PhoneNumber,
            //     type = "text",
            //     text = new { body = message }
            // };
            // 
            // var response = await httpClient.PostAsJsonAsync(
            //     "https://graph.facebook.com/v17.0/YOUR_PHONE_NUMBER_ID/messages",
            //     whatsappPayload);
        }

        #endregion
    }

    #region DTOs

    /// <summary>
    /// Request para envio de notificação WhatsApp
    /// </summary>
    public class WhatsAppNotificationRequest
    {
        [Required(ErrorMessage = "Número de telefone é obrigatório")]
        [Phone(ErrorMessage = "Formato de telefone inválido")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo de mensagem é obrigatório")]
        public string MessageType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Conteúdo da mensagem é obrigatório")]
        [StringLength(1000, ErrorMessage = "Mensagem muito longa (máximo 1000 caracteres)")]
        public string Content { get; set; } = string.Empty;

        public int? HospedeId { get; set; }
        public int? CheckinId { get; set; }
        public string ApartamentoCodigo { get; set; }
        public Dictionary<string, object> TemplateParameters { get; set; }
    }

    /// <summary>
    /// Request do webhook do WhatsApp Business
    /// </summary>
    public class WhatsAppWebhookRequest
    {
        public string MessageId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Error { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    #endregion
}
