using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.Common.Models
{
    public class NotificationInfo
    {
         public string UserId { get; set; }

        public bool IsUserBO { get; set; }

        public string SentByUserId { get; set; }

        public int MotiveId { get; set; }

        public int SubjectId { get; set; }

        public string SubjectMessage { get; set; }

        public string Message { get; set; }

        public DateTime? NotificationDate { get; set; }

        public bool IsRead { get; set; }

        public int OriginId { get; set; }


        public bool SendSMS { get; set; } = true;
        public bool SendEmail { get; set; } = true;
    }
}