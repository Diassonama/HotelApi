using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Conversation : BaseDomainEntity
    {
        public string ConversationId { get; private set; }
        public int UnreadCount { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public int? LastMessageId { get; private set; }

        // Navigation Properties
        public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
        public virtual Message LastMessage { get; set; }

        public Conversation() { }

        public Conversation(string conversationId, List<string> participantIds)
        {
            ConversationId = conversationId;
            UpdatedAt = DateTime.UtcNow;
            UnreadCount = 0;
            DateCreated = DateTime.Now;
            IsActive = true;

            foreach (var participantId in participantIds)
            {
                Participants.Add(new ConversationParticipant(Id, participantId));
            }
        }

        public void UpdateLastMessage(Message message)
        {
            LastMessageId = message.Id;
            UpdatedAt = DateTime.UtcNow;
            LastModifiedDate = DateTime.Now;
        }

        public void IncrementUnreadCount()
        {
            UnreadCount++;
            LastModifiedDate = DateTime.Now;
        }

        public void ResetUnreadCount()
        {
            UnreadCount = 0;
            LastModifiedDate = DateTime.Now;
        }
    }
}