namespace Hotel.Application.DTOs.Messaging
{
    public class StartConversationRequest
    {
        /// <summary>
        /// ID do usuário com quem iniciar a conversa
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Mensagem inicial opcional
        /// </summary>
        public string InitialMessage { get; set; }
    }

    public class ConversationResponse
    {
        public int Id { get; set; }
        public string ConversationId { get; set; }
        public bool IsNew { get; set; }
        public List<ConversationParticipantDto> Participants { get; set; } = new();
        public LastMessageDto LastMessage { get; set; }
        public int UnreadCount { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class ConversationParticipantDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class LastMessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public DateTime Timestamp { get; set; }
        public string MessageType { get; set; }
    }
}