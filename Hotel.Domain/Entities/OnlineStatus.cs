using Hotel.Domain.Common;
using Hotel.Domain.Identity;

namespace Hotel.Domain.Entities
{
    public class OnlineStatus : BaseDomainEntity
    {
        public string UserId { get; private set; }
        public bool IsOnline { get; private set; }
        public DateTime LastSeen { get; private set; }
        public string ConnectionId { get; private set; }

        // Navigation Properties
        public virtual ApplicationUser User { get; set; }

        public OnlineStatus() { }

        public OnlineStatus(string userId)
        {
            UserId = userId;
            IsOnline = false;
            LastSeen = DateTime.UtcNow;
            DateCreated = DateTime.Now;
            IsActive = true;
        }

        public void SetOnline(string connectionId = null)
        {
            IsOnline = true;
            ConnectionId = connectionId;
            LastSeen = DateTime.UtcNow;
            LastModifiedDate = DateTime.Now;
        }

        public void SetOffline()
        {
            IsOnline = false;
            ConnectionId = null;
            LastSeen = DateTime.UtcNow;
            LastModifiedDate = DateTime.Now;
        }

        public void UpdateLastSeen()
        {
            LastSeen = DateTime.UtcNow;
            LastModifiedDate = DateTime.Now;
        }
    }
}