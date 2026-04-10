using Hotel.Domain.Common;
using Hotel.Domain.Identity;

namespace Hotel.Domain.Entities
{
    public class ConversationParticipant : BaseDomainEntity
    {
        public int ConversationId { get; private set; }
        public string UserId { get; private set; }
        public DateTime JoinedAt { get; private set; }
      

        // Navigation Properties
        public virtual Conversation Conversation { get; set; }
        public virtual ApplicationUser User { get; set; }

        public ConversationParticipant() { }

        public ConversationParticipant(int conversationId, string userId)
        {
            ConversationId = conversationId;
            UserId = userId;
            JoinedAt = DateTime.UtcNow;
            IsActive = true;
            DateCreated = DateTime.Now;
        }

        public void Leave()
        {
            IsActive = false;
            LastModifiedDate = DateTime.Now;
        }
    }
}