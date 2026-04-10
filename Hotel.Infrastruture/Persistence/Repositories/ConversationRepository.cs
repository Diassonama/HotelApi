using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class ConversationRepository : RepositoryBase<Conversation>, IConversationRepository
    {
        private readonly GhotelDbContext _context;

        public ConversationRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Conversation> GetByConversationIdAsync(string conversationId)
        {
            return await _context.Conversations
                .Where(c => c.ConversationId == conversationId && c.IsActive)
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.LastMessage)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId)
        {
            return await _context.Conversations
                .Where(c => c.Participants.Any(p => p.UserId == userId && p.IsActive) && c.IsActive)
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.LastMessage)
                    .ThenInclude(m => m.Sender)
                .OrderByDescending(c => c.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Conversation> GetOrCreateConversationAsync(List<string> participantIds)
        {
            var conversationId = await GenerateConversationIdAsync(participantIds);

            var existingConversation = await GetByConversationIdAsync(conversationId);
            if (existingConversation != null)
            {
                return existingConversation;
            }

            // Criar nova conversa
            var newConversation = new Conversation(conversationId, participantIds);
            await _context.Conversations.AddAsync(newConversation);
            await _context.SaveChangesAsync();

            return await GetByConversationIdAsync(conversationId);
        }

        public async Task<string> GenerateConversationIdAsync(List<string> participantIds)
        {
            // Ordenar IDs para garantir consistência
            var sortedIds = participantIds.OrderBy(id => id).ToList();
            var concatenatedIds = string.Join("-", sortedIds);

            // Gerar hash para IDs únicos
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenatedIds));
                var hash = Convert.ToBase64String(hashedBytes).Replace("/", "_").Replace("+", "-");
                return $"conv_{hash[..16]}"; // Limitar tamanho
            }
        }

        public async Task<bool> IsUserInConversationAsync(string conversationId, string userId)
        {
            return await _context.ConversationParticipants
                .AnyAsync(cp => cp.Conversation.ConversationId == conversationId &&
                               cp.UserId == userId &&
                               cp.IsActive);
        }

        public async Task<IEnumerable<Conversation>> GetActiveConversationsAsync(string userId)
        {
            return await _context.Conversations
                .Where(c => c.Participants.Any(p => p.UserId == userId && p.IsActive) &&
                           c.IsActive &&
                           c.Messages.Any())
                .Include(c => c.Participants)
                    .ThenInclude(p => p.User)
                .Include(c => c.LastMessage)
                .OrderByDescending(c => c.UpdatedAt)
                .ToListAsync();
        }

        public async Task<int> GetTotalUnreadConversationsAsync(string userId)
        {
            return await _context.Conversations
                .Where(c => c.Participants.Any(p => p.UserId == userId && p.IsActive) &&
                           c.UnreadCount > 0 &&
                           c.IsActive)
                .CountAsync();
        }

        public async Task UpdateLastMessageAsync(string conversationId, int messageId)
        {
            var conversation = await GetByConversationIdAsync(conversationId);
            if (conversation != null)
            {
                var message = await _context.Messages.FindAsync(messageId);
                if (message != null)
                {
                    conversation.UpdateLastMessage(message);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task<string> GetExistingConversationIdAsync(List<string> participantIds)
{
    if (participantIds.Count != 2)
    {
        return null; // Por enquanto só suportamos conversas diretas (2 pessoas)
    }

    // Buscar conversas onde ambos os usuários são participantes
    var conversations = await _context.ConversationParticipants
        .Where(cp => participantIds.Contains(cp.UserId) && cp.IsActive)
        .GroupBy(cp => cp.ConversationId)
        .Where(g => g.Count() == 2)
        .Select(g => g.Key)
        .ToListAsync();

    // Verificar qual conversa tem exatamente esses participantes
    foreach (var conversationId in conversations)
    {
        var participants = await _context.ConversationParticipants
            .Where(cp => cp.ConversationId == conversationId && cp.IsActive)
            .Select(cp => cp.UserId)
            .ToListAsync();

        if (participants.OrderBy(p => p).SequenceEqual(participantIds.OrderBy(p => p)))
        {
            var conversation = await _context.Conversations
                .Where(c => c.Id == conversationId && c.IsActive)
                .FirstOrDefaultAsync();
            
            return conversation?.ConversationId;
        }
    }

    return null;
}
        
    }
}