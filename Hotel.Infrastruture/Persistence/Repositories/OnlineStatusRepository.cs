using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class OnlineStatusRepository: RepositoryBase<OnlineStatus>, IOnlineStatusRepository
    {
        private readonly GhotelDbContext _context;

        public OnlineStatusRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<OnlineStatus> GetByUserIdAsync(string userId)
        {
            return await _context.OnlineStatuses
                .Where(os => os.UserId == userId)
                .Include(os => os.User)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OnlineStatus>> GetOnlineUsersAsync()
        {
            return await _context.OnlineStatuses
                .Where(os => os.IsOnline && os.IsActive)
                .Include(os => os.User)
                .OrderByDescending(os => os.LastSeen)
                .ToListAsync();
        }

        public async Task SetUserOnlineAsync(string userId, string connectionId = null)
        {
            var status = await GetByUserIdAsync(userId);
            
            if (status == null)
            {
                status = new OnlineStatus(userId);
                await _context.OnlineStatuses.AddAsync(status);
            }
            
            status.SetOnline(connectionId);
            await _context.SaveChangesAsync();
        }

        public async Task SetUserOfflineAsync(string userId)
        {
            var status = await GetByUserIdAsync(userId);
            if (status != null)
            {
                status.SetOffline();
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateLastSeenAsync(string userId)
        {
            var status = await GetByUserIdAsync(userId);
            if (status != null)
            {
                status.UpdateLastSeen();
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsUserOnlineAsync(string userId)
        {
            var status = await GetByUserIdAsync(userId);
            return status?.IsOnline ?? false;
        }

        public async Task<IEnumerable<OnlineStatus>> GetUsersByStatusAsync(bool isOnline)
        {
            return await _context.OnlineStatuses
                .Where(os => os.IsOnline == isOnline && os.IsActive)
                .Include(os => os.User)
                .OrderByDescending(os => os.LastSeen)
                .ToListAsync();
        }

        public async Task<Dictionary<string, bool>> GetUsersOnlineStatusAsync(List<string> userIds)
        {
            var statuses = await _context.OnlineStatuses
                .Where(os => userIds.Contains(os.UserId) && os.IsActive)
                .ToListAsync();

            var result = new Dictionary<string, bool>();
            foreach (var userId in userIds)
            {
                var status = statuses.FirstOrDefault(s => s.UserId == userId);
                result[userId] = status?.IsOnline ?? false;
            }

            return result;
        }

        public async Task CleanupStaleConnectionsAsync(TimeSpan threshold)
        {
            var cutoffTime = DateTime.UtcNow.Subtract(threshold);
            
            var staleStatuses = await _context.OnlineStatuses
                .Where(os => os.IsOnline && os.LastSeen < cutoffTime)
                .ToListAsync();

            foreach (var status in staleStatuses)
            {
                status.SetOffline();
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetOnlineUserCountAsync()
        {
            return await _context.OnlineStatuses
                .Where(os => os.IsOnline && os.IsActive)
                .CountAsync();
        }

        public async Task UpdateConnectionIdAsync(string userId, string connectionId)
        {
            var status = await GetByUserIdAsync(userId);
            if (status != null && status.IsOnline)
            {
                status.SetOnline(connectionId);
                await _context.SaveChangesAsync();
            }
        }
    }
}