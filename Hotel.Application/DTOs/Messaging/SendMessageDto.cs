using Hotel.Domain.Entities;

namespace Hotel.Application.DTOs.Messaging
{
    public class SendMessageDto
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public MessageType MessageType { get; set; } = MessageType.Text;
        public string AttachmentUrl { get; set; }
    }

    public class MessageResponseDto
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public MessageType MessageType { get; set; }
        public string AttachmentUrl { get; set; }
        public string ConversationId { get; set; }
    }

    public class ConversationDto
    {
        public int Id { get; set; }
        public string ConversationId { get; set; }
        public List<ParticipantDto> Participants { get; set; } = new();
        public MessageResponseDto LastMessage { get; set; }
        public int UnreadCount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ParticipantDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}