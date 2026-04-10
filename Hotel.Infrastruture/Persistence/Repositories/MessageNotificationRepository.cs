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
    public class MessageNotificationRepository : RepositoryBase<MessageNotification>, IMessageNotificationRepository
    {
        private readonly GhotelDbContext _context;

        public MessageNotificationRepository(GhotelDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<MessageNotification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.MessageNotifications
                .Where(mn => mn.ReceiverId == userId && mn.IsActive)
                .Include(mn => mn.Message)
                .Include(mn => mn.Sender)
                .Include(mn => mn.Receiver)
                .OrderByDescending(mn => mn.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<MessageNotification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _context.MessageNotifications
                .Where(mn => mn.ReceiverId == userId && !mn.IsRead && mn.IsActive)
                .Include(mn => mn.Message)
                .Include(mn => mn.Sender)
                .OrderByDescending(mn => mn.Timestamp)
                .ToListAsync();
        }

        public async Task<int> GetUnreadNotificationCountAsync(string userId)
        {
            return await _context.MessageNotifications
                .Where(mn => mn.ReceiverId == userId && !mn.IsRead && mn.IsActive)
                .CountAsync();
        }

        public async Task<IEnumerable<MessageNotification>> GetRecentNotificationsAsync(string userId, int count = 10)
        {
            return await _context.MessageNotifications
                .Where(mn => mn.ReceiverId == userId && mn.IsActive)
                .Include(mn => mn.Message)
                .Include(mn => mn.Sender)
                .OrderByDescending(mn => mn.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _context.MessageNotifications.FindAsync(notificationId);
            if (notification != null && !notification.IsRead)
            {
                notification.MarkAsRead();
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllNotificationsAsReadAsync(string userId)
        {
            var unreadNotifications = await _context.MessageNotifications
                .Where(mn => mn.ReceiverId == userId && !mn.IsRead && mn.IsActive)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.MarkAsRead();
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.MessageNotifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsActive = false;
                notification.LastModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllUserNotificationsAsync(string userId)
        {
            var userNotifications = await _context.MessageNotifications
                .Where(mn => mn.ReceiverId == userId && mn.IsActive)
                .ToListAsync();

            foreach (var notification in userNotifications)
            {
                notification.IsActive = false;
                notification.LastModifiedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<MessageNotification> GetNotificationByIdAsync(int notificationId)
        {
            return await _context.MessageNotifications
                .Include(mn => mn.Message)
                .Include(mn => mn.Sender)
                .Include(mn => mn.Receiver)
                .Where(mn => mn.Id == notificationId && mn.IsActive)
                .FirstOrDefaultAsync();
        }
    }
}