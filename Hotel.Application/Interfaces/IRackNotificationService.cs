using Hotel.Application.DTOs.Dashboard;
using Hotel.Domain.Entities;

namespace Hotel.Application.Interfaces
{
    /// <summary>
    /// Interface para o serviço de notificações do rack
    /// </summary>
    public interface IRackNotificationService
    {
        /// <summary>
        /// Notifica sobre um novo check-in
        /// </summary>
        Task NotifyCheckinAsync(Checkins checkin);

        /// <summary>
        /// Notifica sobre um check-out
        /// </summary>
        Task NotifyCheckoutAsync(Checkins checkin);

        /// <summary>
        /// Notifica sobre mudança de status do apartamento
        /// </summary>
        Task NotifyApartmentStatusChangeAsync(Hotel.Domain.Entities.Apartamentos apartamento);

        /// <summary>
        /// Notifica sobre alteração na hospedagem
        /// </summary>
        Task NotifyHospedagemUpdateAsync(Hotel.Domain.Entities.Hospedagem hospedagem);

        /// <summary>
        /// Notifica sobre novo pagamento
        /// </summary>
        Task NotifyPaymentAsync(Hotel.Domain.Entities.Pagamento pagamento);

        /// <summary>
        /// Notifica atualização geral do rack
        /// </summary>
        Task NotifyRackUpdateAsync();

        /// <summary>
        /// Notifica sobre apartamentos ocupados
        /// </summary>
        Task NotifyApartamentosOcupadosAsync(IEnumerable<ApartamentoOcupadoDto> apartamentos);

        /// <summary>
        /// Notifica sobre métricas do dashboard
        /// </summary>
        Task NotifyDashboardMetricsAsync(object metrics);

        /// <summary>
        /// Notifica sobre erro ou alerta
        /// </summary>
        Task NotifyErrorAsync(string message, string details = null);

        /// <summary>
        /// Notifica sobre informação geral
        /// </summary>
        Task NotifyInfoAsync(string message, object data = null);
    }
}
