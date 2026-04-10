namespace Hotel.Application.DTOs
{
    public class MessageNotificationDto
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverId { get; set; }
        public bool IsNew { get; set; }
        public bool IsRead { get; set; }
        public DateTime Timestamp { get; set; }
        public string MessagePreview { get; set; }
        public MessageNotificationType NotificationType { get; set; }
    }

    public class NotificationCountDto
    {
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
        public int NewCount { get; set; }
    }

    public class MarkNotificationDto
    {
        public List<int> NotificationIds { get; set; } = new();
        public bool IsRead { get; set; }
    }

    public enum MessageNotificationType
    {
        NewMessage = 1,
        MessageRead = 2,
        MessageDelivered = 3
    }
}