using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Apartamento.Commands.Notifications
{
    public class ApartamentoCreateSMSHandler : INotificationHandler<ApartamentoCreateNotification>
    {
       private readonly ILogger<ApartamentoCreateSMSHandler>  _logger;

        public ApartamentoCreateSMSHandler(ILogger<ApartamentoCreateSMSHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ApartamentoCreateNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"SMS sent do apartamento: {notification.apartamentos.Codigo}");
           return Task.CompletedTask;
        }
    }
}