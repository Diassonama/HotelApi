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
    public class ConversationParticipantRepository : RepositoryBase<ConversationParticipant>, IConversationParticipantRepository
    {
         private readonly GhotelDbContext _context;
        public ConversationParticipantRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }
 public async Task<IEnumerable<ConversationParticipant>> GetConversationParticipantsAsync(string conversationId)
        {

            var conversationInternalId = await _context.Conversations
        .Where(c => c.ConversationId == conversationId)
        .Select(c => c.Id)
        .FirstOrDefaultAsync();

    if (conversationInternalId == 0)
        return new List<ConversationParticipant>();
            return await _context.ConversationParticipants
                .Where(cp => cp.Conversation.ConversationId == conversationId && cp.IsActive)
                .Include(cp => cp.User)

                .Include(cp => cp.Conversation)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConversationParticipant>> GetUserParticipationsAsync(string userId)
        {
            return await _context.ConversationParticipants
                .Where(cp => cp.UserId == userId && cp.IsActive)
                .Include(cp => cp.Conversation)
                    .ThenInclude(c => c.LastMessage)
                .ToListAsync();
        }



        public async Task<ConversationParticipant> GetParticipantAsync(string conversationId, string userId)
        {
            return await _context.ConversationParticipants
                .Where(cp => cp.Conversation.ConversationId == conversationId &&
                            cp.UserId == userId)
                .Include(cp => cp.User)
                .Include(cp => cp.Conversation)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsUserParticipantAsync(string conversationId, string userId)
        {
            return await _context.ConversationParticipants
                .AnyAsync(cp => cp.Conversation.ConversationId == conversationId && 
                               cp.UserId == userId && 
                               cp.IsActive);
        }

        public async Task AddParticipantAsync(string conversationId, string userId)
        {
            var conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.ConversationId == conversationId);

            if (conversation != null)
            {
                var existingParticipant = await GetParticipantAsync(conversationId, userId);
                if (existingParticipant == null)
                {
                    var participant = new ConversationParticipant(conversation.Id, userId);
                    await _context.ConversationParticipants.AddAsync(participant);
                }
                else if (!existingParticipant.IsActive)
                {
                    existingParticipant.IsActive = true;
                    existingParticipant.LastModifiedDate = DateTime.Now;
                }
                
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveParticipantAsync(string conversationId, string userId)
        {
            var participant = await GetParticipantAsync(conversationId, userId);
            if (participant != null && participant.IsActive)
            {
                participant.Leave();
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetParticipantCountAsync(string conversationId)
        {
            return await _context.ConversationParticipants
                .Where(cp => cp.Conversation.ConversationId == conversationId && cp.IsActive)
                .CountAsync();
        }
    }
}