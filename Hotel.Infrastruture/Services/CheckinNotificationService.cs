using Hotel.Application.Interfaces;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Microsoft.Extensions.Logging;

namespace Hotel.Infrastruture.Services
{
    /// <summary>
    /// Serviço para integrar notificações do rack com operações de check-in/check-out
    /// </summary>
    public class CheckinNotificationService
    {
        private readonly IRackNotificationService _rackNotificationService;
        private readonly ICheckinRepository _checkinRepository;
        private readonly ILogger<CheckinNotificationService> _logger;

        public CheckinNotificationService(
            IRackNotificationService rackNotificationService,
            ICheckinRepository checkinRepository,
            ILogger<CheckinNotificationService> logger)
        {
            _rackNotificationService = rackNotificationService;
            _checkinRepository = checkinRepository;
            _logger = logger;
        }

        /// <summary>
        /// Processa um novo check-in e envia notificações
        /// </summary>
        public async Task ProcessCheckinAsync(Checkins checkin)
        {
            try
            {
                // Aqui você chamaria o repositório para salvar o check-in
                // await _checkinRepository.AddAsync(checkin);

                // Notifica sobre o novo check-in
                await _rackNotificationService.NotifyCheckinAsync(checkin);

                // Notifica atualização geral do rack
                await _rackNotificationService.NotifyRackUpdateAsync();

                var apartamentoCodigos = string.Join(", ", checkin.apartamentos?.Select(a => a.Codigo) ?? new string[] { });
                _logger.LogInformation($"Check-in processado e notificações enviadas para apartamentos {apartamentoCodigos}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar check-in: {ex.Message}");
                await _rackNotificationService.NotifyErrorAsync("Erro no check-in", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Processa um check-out e envia notificações
        /// </summary>
        public async Task ProcessCheckoutAsync(Checkins checkin)
        {
            try
            {
                // Atualiza a data de saída
                // checkin.DataSaida = DateTime.Now; // Esta propriedade tem setter privado

                // Aqui você chamaria o repositório para atualizar o check-in
                // await _checkinRepository.UpdateAsync(checkin);

                // Notifica sobre o check-out
                await _rackNotificationService.NotifyCheckoutAsync(checkin);

                // Notifica atualização geral do rack
                await _rackNotificationService.NotifyRackUpdateAsync();

                var apartamentoCodigos = string.Join(", ", checkin.apartamentos?.Select(a => a.Codigo) ?? new string[] { });
                _logger.LogInformation($"Check-out processado e notificações enviadas para apartamentos {apartamentoCodigos}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar check-out: {ex.Message}");
                await _rackNotificationService.NotifyErrorAsync("Erro no check-out", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Atualiza o status de um apartamento e envia notificações
        /// </summary>
        public async Task UpdateApartmentStatusAsync(Hotel.Domain.Entities.Apartamentos apartamento)
        {
            try
            {
                // Aqui você chamaria o repositório para atualizar o apartamento
                // await _apartamentoRepository.UpdateAsync(apartamento);

                // Notifica sobre a mudança de status
                await _rackNotificationService.NotifyApartmentStatusChangeAsync(apartamento);

                // Notifica atualização geral do rack
                await _rackNotificationService.NotifyRackUpdateAsync();

                _logger.LogInformation($"Status do apartamento {apartamento.Codigo} atualizado para {apartamento.Situacao}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar status do apartamento: {ex.Message}");
                await _rackNotificationService.NotifyErrorAsync("Erro na atualização do apartamento", ex.Message);
                throw;
            }
        }
    }
}
