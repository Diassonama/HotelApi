using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.Models;
using Hotel.Application.Interfaces.Services;

namespace Hotel.Infrastruture.Services
{
    public class NotificationService : INotificationService
    {
        public Task<bool> SendNotification(NotificationInfo info, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}