using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IMessageNotificationRepository : IRepositoryBase<MessageNotification>
    {
        Task<IEnumerable<MessageNotification>> GetUserNotificationsAsync(string userId);
        Task<IEnumerable<MessageNotification>> GetUnreadNotificationsAsync(string userId);
        Task<int> GetUnreadNotificationCountAsync(string userId);
        Task MarkNotificationAsReadAsync(int notificationId);
        Task MarkAllNotificationsAsReadAsync(string userId);
        Task<IEnumerable<MessageNotification>> GetRecentNotificationsAsync(string userId, int count = 10);
        Task DeleteNotificationAsync(int notificationId);
        Task<MessageNotification> GetNotificationByIdAsync(int notificationId);
    }
}