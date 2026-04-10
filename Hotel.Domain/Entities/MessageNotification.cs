using Hotel.Domain.Common;
using Hotel.Domain.Identity;

namespace Hotel.Domain.Entities
{
    public class MessageNotification : BaseDomainEntity
    {
        public int MessageId { get; private set; }
        public string SenderId { get; private set; }
        public string ReceiverId { get; private set; }
        public bool IsNew { get; private set; }
        public DateTime Timestamp { get; private set; }
        public bool IsRead { get; private set; }

        // Navigation Properties
        public virtual Message Message { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }

        public MessageNotification() { }

        public MessageNotification(int messageId, string senderId, string receiverId)
        {
            MessageId = messageId;
            SenderId = senderId;
            ReceiverId = receiverId;
            IsNew = true;
            IsRead = false;
            Timestamp = DateTime.UtcNow;
            DateCreated = DateTime.Now;
            IsActive = true;
        }

        public void MarkAsRead()
        {
            IsRead = true;
            IsNew = false;
            LastModifiedDate = DateTime.Now;
        }
    }
}