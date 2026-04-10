
using Hotel.Application.Common.Models;

namespace Hotel.Application.Interfaces.Services
{
    public interface INotificationService
    {
        Task<bool> SendNotification(NotificationInfo info, CancellationToken cancellationToken);
    }
}