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
    public class MessageRepository : RepositoryBase<Message>, IMessageRepository
    {
        private readonly GhotelDbContext _context;

        public MessageRepository(GhotelDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

     public async Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(string conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId && m.IsActive)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(string userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == userId && !m.IsRead && m.IsActive)
                .Include(m => m.Sender)
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<Message> GetLastMessageByConversationIdAsync(string conversationId)
        {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId && m.IsActive)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetUnreadCountByConversationIdAsync(string conversationId, string userId)
        {
            return await _context.Messages
                .Where(m => m.ConversationId == conversationId && 
                           m.ReceiverId == userId && 
                           !m.IsRead && 
                           m.IsActive)
                .CountAsync();
        }

        public async Task MarkMessagesAsReadAsync(string conversationId, string userId)
        {
            var unreadMessages = await _context.Messages
                .Where(m => m.ConversationId == conversationId && 
                           m.ReceiverId == userId && 
                           !m.IsRead && 
                           m.IsActive)
                .ToListAsync();

            foreach (var message in unreadMessages)
            {
                message.MarkAsRead();
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(string senderId, string receiverId, int page = 1, int pageSize = 50)
        {
            var skip = (page - 1) * pageSize;

            return await _context.Messages
                .Where(m => ((m.SenderId == senderId && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == senderId)) &&
                           m.IsActive)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.Timestamp)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetRecentMessagesAsync(string userId, int count = 10)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == userId || m.ReceiverId == userId) && m.IsActive)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.Timestamp)
                .Take(count)
                .ToListAsync();
        }

        public async Task<int> GetTotalUnreadCountAsync(string userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == userId && !m.IsRead && m.IsActive)
                .CountAsync();
        }

        public async Task<IEnumerable<Message>> SearchMessagesAsync(string userId, string searchTerm)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == userId || m.ReceiverId == userId) &&
                           m.Content.Contains(searchTerm) &&
                           m.IsActive)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();
        }
    }
}