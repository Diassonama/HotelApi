using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IMessageRepository : IRepositoryBase<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByConversationIdAsync(string conversationId);
        Task<IEnumerable<Message>> GetUnreadMessagesAsync(string userId);
        Task<Message> GetLastMessageByConversationIdAsync(string conversationId);
        Task<int> GetUnreadCountByConversationIdAsync(string conversationId, string userId);
        Task MarkMessagesAsReadAsync(string conversationId, string userId);
        Task<IEnumerable<Message>> GetMessagesBetweenUsersAsync(string senderId, string receiverId, int page = 1, int pageSize = 50);

        
    }
}