using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IOnlineStatusRepository : IRepositoryBase<OnlineStatus>
    {
        Task<OnlineStatus> GetByUserIdAsync(string userId);
        Task<IEnumerable<OnlineStatus>> GetOnlineUsersAsync();
        Task SetUserOnlineAsync(string userId, string connectionId = null);
        Task SetUserOfflineAsync(string userId);
        Task UpdateLastSeenAsync(string userId);
        Task<bool> IsUserOnlineAsync(string userId);
    }
}