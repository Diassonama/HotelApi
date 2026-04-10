using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class UserNotifications
    {
       public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Utilizador AspNetUserTo { get; set; }

        public string SentByUserId { get; set; }
        [ForeignKey("SentByUserId")]
        public virtual Utilizador AspNetUserFrom { get; set; }

        public int MotiveId { get; set; }
        [ForeignKey("MotiveId")]
      //  public virtual NotificationMotives NotificationMotive { get; set; }

        public int SubjectId { get; set; }
        [ForeignKey("SubjectId")]
     //   public virtual NotificationSubjects NotificationSubject { get; set; }

        public string Message { get; set; }

        public DateTime? NotificationDate { get; set; }
        
        public bool IsRead { get; set; }

        public int OriginId { get; set; }
      //  [ForeignKey("OriginId")]
     //   public virtual Origin Origin { get; set; }       

    }
}