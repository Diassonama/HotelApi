using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Hotel.Application.DTOs.Dashboard;

namespace Hotel.Api.Hubs
{
    /// <summary>
    /// Hub SignalR para notificações em tempo real do rack do hotel
    /// </summary>
    [Authorize]
    public class RackHub : Hub
    {
        private readonly ILogger<RackHub> _logger;

        public RackHub(ILogger<RackHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Evento chamado quando um cliente se conecta
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userIdentifier = Context.UserIdentifier;
            
            _logger.LogInformation($"Cliente conectado ao RackHub. ConnectionId: {connectionId}, User: {userIdentifier}");
            
            // Adiciona o cliente ao grupo geral do rack
            await Groups.AddToGroupAsync(connectionId, "RackGroup");
            
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Evento chamado quando um cliente se desconecta
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            var userIdentifier = Context.UserIdentifier;
            
            _logger.LogInformation($"Cliente desconectado do RackHub. ConnectionId: {connectionId}, User: {userIdentifier}");
            
            if (exception != null)
            {
                _logger.LogError(exception, $"Erro na desconexão: {exception.Message}");
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Permite que um cliente se junte a um grupo específico (ex: por andar, tipo de apartamento)
        /// </summary>
        /// <param name="groupName">Nome do grupo</param>
        public async Task JoinGroup(string groupName)
        {
            var connectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(connectionId, groupName);
            
            _logger.LogInformation($"Cliente {connectionId} adicionado ao grupo {groupName}");
            
            await Clients.Caller.SendAsync("JoinedGroup", groupName);
        }

        /// <summary>
        /// Permite que um cliente saia de um grupo específico
        /// </summary>
        /// <param name="groupName">Nome do grupo</param>
        public async Task LeaveGroup(string groupName)
        {
            var connectionId = Context.ConnectionId;
            await Groups.RemoveFromGroupAsync(connectionId, groupName);
            
            _logger.LogInformation($"Cliente {connectionId} removido do grupo {groupName}");
            
            await Clients.Caller.SendAsync("LeftGroup", groupName);
        }

        /// <summary>
        /// Método para teste de conexão
        /// </summary>
        /// <param name="message">Mensagem de teste</param>
        public async Task SendTestMessage(string message)
        {
            var connectionId = Context.ConnectionId;
            var userIdentifier = Context.UserIdentifier;
            
            _logger.LogInformation($"Mensagem de teste recebida de {userIdentifier}: {message}");
            
            await Clients.All.SendAsync("TestMessage", $"Echo from {userIdentifier}: {message}");
        }
    }
}
