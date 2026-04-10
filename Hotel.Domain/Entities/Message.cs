using Hotel.Domain.Common;
using Hotel.Domain.Identity;

namespace Hotel.Domain.Entities
{
    public class Message : BaseDomainEntity
    {
        public string SenderId { get; private set; }
        public string ReceiverId { get; private set; }
        public string Content { get; private set; }
        public DateTime Timestamp { get; private set; }
        public bool IsRead { get; private set; }
        public MessageType MessageType { get; private set; }
        public string AttachmentUrl { get; private set; }
        public string ConversationId { get; private set; }

        // Navigation Properties
         public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
        public virtual Conversation Conversation { get; set; } 

        public Message() { }

        public Message(string senderId, string receiverId, string content, MessageType messageType, string conversationId)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
            Content = content;
            MessageType = messageType;
            ConversationId = conversationId;
            Timestamp = DateTime.UtcNow;
            IsRead = false;
            DateCreated = DateTime.Now;
            IsActive = true;
        }

        public void MarkAsRead()
        {
            IsRead = true;
            LastModifiedDate = DateTime.Now;
        }

        public void AddAttachment(string attachmentUrl)
        {
            AttachmentUrl = attachmentUrl;
            LastModifiedDate = DateTime.Now;
        }
    }

    public enum MessageType
    {
        Text = 1,
        File = 2,
        Image = 3
    }
}