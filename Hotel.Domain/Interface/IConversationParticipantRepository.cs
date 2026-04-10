using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IConversationParticipantRepository : IRepositoryBase<ConversationParticipant>
    {
        Task<IEnumerable<ConversationParticipant>> GetConversationParticipantsAsync(string conversationId);
        Task<IEnumerable<ConversationParticipant>> GetUserParticipationsAsync(string userId);
        Task<ConversationParticipant> GetParticipantAsync(string conversationId, string userId);
        Task<bool> IsUserParticipantAsync(string conversationId, string userId);
        Task AddParticipantAsync(string conversationId, string userId);
        Task RemoveParticipantAsync(string conversationId, string userId);
        Task<int> GetParticipantCountAsync(string conversationId);
    }
}