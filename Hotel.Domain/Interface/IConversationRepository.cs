using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IConversationRepository : IRepositoryBase<Conversation>
    {
        Task<Conversation> GetByConversationIdAsync(string conversationId);
        Task<IEnumerable<Conversation>> GetUserConversationsAsync(string userId);
        Task<Conversation> GetOrCreateConversationAsync(List<string> participantIds);
        Task<string> GenerateConversationIdAsync(List<string> participantIds);
        Task<bool> IsUserInConversationAsync(string conversationId, string userId);
        Task<string> GetExistingConversationIdAsync(List<string> participantIds);

    }
}