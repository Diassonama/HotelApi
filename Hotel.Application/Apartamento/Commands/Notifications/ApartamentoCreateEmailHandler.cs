using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Apartamento.Commands.Notifications
{
    public class ApartamentoCreateEmailHandler : INotificationHandler<ApartamentoCreateNotification>
    {
        private ILogger<ApartamentoCreateEmailHandler> _logger;

        public ApartamentoCreateEmailHandler(ILogger<ApartamentoCreateEmailHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(ApartamentoCreateNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($" Email sent do apartamento: {notification.apartamentos.Codigo}");

            return Task.CompletedTask;
        }
    }
}