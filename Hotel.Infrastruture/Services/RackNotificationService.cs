using Hotel.Application.DTOs.Dashboard;
using Hotel.Application.Interfaces;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Hotel.Infrastruture.Services
{
    /// <summary>
    /// Serviço para gerenciar notificações em tempo real do rack via SignalR
    /// </summary>
    public class RackNotificationService : IRackNotificationService
    {
        private readonly IHubContext<Hub> _hubContext;
        private readonly ILogger<RackNotificationService> _logger;

        public RackNotificationService(
            IHubContext<Hub> hubContext,
            ILogger<RackNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// Notifica sobre um novo check-in
        /// </summary>
        public async Task NotifyCheckinAsync(Checkins checkin)
        {
            try
            {
                var notificationData = new
                {
                    Type = "CHECKIN",
                    CheckinId = checkin.Id,
                    ApartamentoIds = checkin.apartamentos?.Select(a => a.Id).ToList(),
                    ApartamentosCodigos = checkin.apartamentos?.Select(a => a.Codigo).ToList(),
                    DataEntrada = checkin.DataEntrada,
                    Hospedes = checkin.Hospedes?.Select(h => new
                    {
                        Id = h.Id,
                        Nome = h.Clientes?.Nome,
                        Telefone = h.Clientes?.Telefone
                    }).ToList(),
                    Timestamp = DateTime.Now
                };

                await _hubContext.Clients.Group("RackGroup").SendAsync("CheckinUpdate", notificationData);
                await _hubContext.Clients.All.SendAsync("RackUpdate", notificationData);

                var apartamentoCodigos = string.Join(", ", checkin.apartamentos?.Select(a => a.Codigo) ?? new string[] { });
                _logger.LogInformation($"Notificação de check-in enviada para apartamentos {apartamentoCodigos}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar notificação de check-in: {ex.Message}");
            }
        }

        /// <summary>
        /// Notifica sobre um check-out
        /// </summary>
        public async Task NotifyCheckoutAsync(Checkins checkin)
        {
            try
            {
                var notificationData = new
                {
                    Type = "CHECKOUT",
                    CheckinId = checkin.Id,
                    ApartamentoIds = checkin.apartamentos?.Select(a => a.Id).ToList(),
                    ApartamentosCodigos = checkin.apartamentos?.Select(a => a.Codigo).ToList(),
                    DataSaida = checkin.DataSaida,
                    Hospedes = checkin.Hospedes?.Select(h => new
                    {
                        Id = h.Id,
                        Nome = h.Clientes?.Nome,
                        Telefone = h.Clientes?.Telefone
                    }).ToList(),
                    Timestamp = DateTime.Now
                };

                await _hubContext.Clients.Group("RackGroup").SendAsync("CheckoutUpdate", notificationData);
                await _hubContext.Clients.All.SendAsync("RackUpdate", notificationData);

                var apartamentoCodigos = string.Join(", ", checkin.apartamentos?.Select(a => a.Codigo) ?? new string[] { });
                _logger.LogInformation($"Notificação de check-out enviada para apartamentos {apartamentoCodigos}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar notificação de check-out: {ex.Message}");
            }
        }

        /// <summary>
        /// Notifica sobre mudança de status do apartamento
        /// </summary>
        public async Task NotifyApartmentStatusChangeAsync(Hotel.Domain.Entities.Apartamentos apartamento)
        {
            try
            {
                var notificationData = new
                {
                    Type = "APARTMENT_STATUS_CHANGE",
                    ApartamentoId = apartamento.Id,
                    Codigo = apartamento.Codigo,
                    Observacao = apartamento.Observacao,
                    Situacao = apartamento.Situacao.ToString(),
                    CheckinsId = apartamento.CheckinsId,
                    Timestamp = DateTime.Now
                };

                await _hubContext.Clients.Group("RackGroup").SendAsync("ApartmentStatusUpdate", notificationData);
                await _hubContext.Clients.All.SendAsync("RackUpdate", notificationData);

                _logger.LogInformation($"Notificação de mudança de status enviada para apartamento {apartamento.Codigo} - Status: {apartamento.Situacao}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar notificação de mudança de status: {ex.Message}");
            }
        }

        /// <summary>
        /// Notifica sobre alteração na hospedagem
        /// </summary>
        public async Task NotifyHospedagemUpdateAsync(Hotel.Domain.Entities.Hospedagem hospedagem)
        {
            try
            {
                var notificationData = new
                {
                    Type = "HOSPEDAGEM_UPDATE",
                    HospedagemId = hospedagem.Id,
                    CheckinsId = hospedagem.CheckinsId,
                    DataAbertura = hospedagem.DataAbertura,
                    PrevisaoFechamento = hospedagem.PrevisaoFechamento,
                    QuantidadeHomens = hospedagem.QuantidadeHomens,
                    QuantidadeMulheres = hospedagem.QuantidadeMulheres,
                    QuantidadeCrianca = hospedagem.QuantidadeCrianca,
                    EmpresasId = hospedagem.EmpresasId,
                    Timestamp = DateTime.Now
                };

                await _hubContext.Clients.Group("RackGroup").SendAsync("HospedagemUpdate", notificationData);
                await _hubContext.Clients.All.SendAsync("RackUpdate", notificationData);

                _logger.LogInformation($"Notificação de alteração de hospedagem enviada - ID: {hospedagem.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar notificação de hospedagem: {ex.Message}");
            }
        }

        /// <summary>
        /// Notifica sobre novo pagamento
        /// </summary>
        public async Task NotifyPaymentAsync(Hotel.Domain.Entities.Pagamento pagamento)
        {
            try
            {
                var notificationData = new
                {
                    Type = "PAYMENT",
                    PagamentoId = pagamento.Id,
                    CheckinsId = pagamento.OrigemId,
                    Valor = pagamento.Valor,
                    DataVencimento = pagamento.DataVencimento,
                    Timestamp = DateTime.Now
                };

                await _hubContext.Clients.Group("RackGroup").SendAsync("PaymentUpdate", notificationData);
                await _hubContext.Clients.All.SendAsync("RackUpdate", notificationData);

                _logger.LogInformation($"Notificação de pagamento enviada - Valor: {pagamento.Valor}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar notificação de pagamento: {ex.Message}");
            }
        }

        /// <summary>
        /// Notifica atualização geral do rack
        /// </summary>
        public async Task NotifyRackUpdateAsync()
        {
            try
            {
                var notificationData = new
                {
                    Type = "RACK_GENERAL_UPDATE",
                    Message = "Rack atualizado",
                    Timestamp = DateTime.Now
                };

                await _hubContext.Clients.Group("RackGroup").SendAsync("GeneralRackUpdate", notificationData);
                await _hubContext.Clients.All.SendAsync("RackUpdate", notificationData);

                _logger.LogInformation("Notificação geral de atualização do rack enviada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar notificação geral do rack: {ex.Message}");
            }
        }

        /// <summary>
        /// Notifica sobre apartamentos ocupados
        /// </summary>
        public async Task NotifyApartamentosOcupadosAsync(IEnumerable<ApartamentoOcupadoDto> apartamentos)
        {
            try
            {
                var notificationData = new
                {
                    Type = "APARTAMENTOS_OCUPADOS",
                    Apartamentos = apartamentos,
                    Count = apartamentos.Count(),
                    Timestamp = DateTime.Now
                };

                await _hubContext.Clients.Group("RackGroup").SendAsync("ApartamentosOcupadosUpdate", notificationData);

                _logger.LogInformation($"Notificação de apartamentos ocupados enviada - {apartamentos.Count()} apartamentos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar notificação de apartamentos ocupados: {ex.Message}");
            }
        }

        /// <summary>
        /// Notifica sobre métricas do dashboard
        /// </summary>
        public async Task NotifyDashboardMetricsAsync(object metrics)
        {
            try
            {
                var notificationData = new
                {
                    Type = "DASHBOARD_METRICS",
                    Metrics = metrics,
                    Timestamp = DateTime.Now
                };

                await _hubContext.Clients.Group("RackGroup").SendAsync("DashboardMetricsUpdate", notificationData);

                _logger.LogInformation("Notificação de métricas do dashboard enviada");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar notificação de métricas: {ex.Message}");
            }
        }

        /// <summary>
        /// Notifica sobre erro ou alerta
        /// </summary>
        public async Task NotifyErrorAsync(string message, string details = null)
        {
            try
            {
                var notificationData = new
                {
                    Type = "ERROR",
                    Message = message,
                    Details = details,
                    Timestamp = DateTime.Now
                };

                await _hubContext.Clients.Group("RackGroup").SendAsync("ErrorNotification", notificationData);

                _logger.LogWarning($"Notificação de erro enviada: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar notificação de erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Notifica sobre informação geral
        /// </summary>
        public async Task NotifyInfoAsync(string message, object data = null)
        {
            try
            {
                var notificationData = new
                {
                    Type = "INFO",
                    Message = message,
                    Data = data,
                    Timestamp = DateTime.Now
                };

                await _hubContext.Clients.Group("RackGroup").SendAsync("InfoNotification", notificationData);

                _logger.LogInformation($"Notificação de informação enviada: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao enviar notificação de informação: {ex.Message}");
            }
        }
    }
}
