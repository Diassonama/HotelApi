using Hotel.Application.DTOs.Messaging;
using Hotel.Application.Services;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UsuarioLogado _logado;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(
            IUnitOfWork unitOfWork, 
            UsuarioLogado logado,
            ILogger<MessagesController> logger)
        {
            _unitOfWork = unitOfWork;
            _logado = logado;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto messageDto)
        {
            try
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];
                _logger.LogInformation("📨 [MESSAGE-{CorrelationId}] Enviando mensagem de {SenderId} para {ReceiverId}", 
                    correlationId, messageDto.SenderId, messageDto.ReceiverId);

                // Gerar ID da conversa
                var conversationId = await _unitOfWork.Conversations.GenerateConversationIdAsync(
                    new List<string> { messageDto.SenderId, messageDto.ReceiverId });

                // Obter ou criar conversa
                var conversation = await _unitOfWork.Conversations.GetOrCreateConversationAsync(
                    new List<string> { messageDto.SenderId, messageDto.ReceiverId });

                // Criar mensagem
                var message = new Hotel.Domain.Entities.Message(
                    messageDto.SenderId,
                    messageDto.ReceiverId,
                    messageDto.Content,
                    messageDto.MessageType,
                    conversationId);

                if (!string.IsNullOrEmpty(messageDto.AttachmentUrl))
                {
                    message.AddAttachment(messageDto.AttachmentUrl);
                }

                await _unitOfWork.Messages.Add(message);

                // Atualizar conversa
                conversation.UpdateLastMessage(message);
                conversation.IncrementUnreadCount();
                await _unitOfWork.Conversations.Update(conversation);

                // Criar notificação
                var notification = new Hotel.Domain.Entities.MessageNotification(
                    message.Id, messageDto.SenderId, messageDto.ReceiverId);
                await _unitOfWork.MessageNotifications.Add(notification);

                await _unitOfWork.Save();

                _logger.LogInformation("✅ [MESSAGE-{CorrelationId}] Mensagem enviada com sucesso: ID={MessageId}", 
                    correlationId, message.Id);

                return Ok(new
                {
                    Id = message.Id,
                    ConversationId = conversationId,
                    Timestamp = message.Timestamp,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao enviar mensagem: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao enviar mensagem", Details = ex.Message });
            }
        }

        [HttpGet("conversation/{conversationId}")]
        public async Task<IActionResult> GetConversationMessages(string conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                 var correlationId = Guid.NewGuid().ToString("N")[..8];
            _logger.LogInformation("📖 [MESSAGE-{CorrelationId}] Obtendo mensagens da conversa {ConversationId} para usuário {UserId} - Página {Page}, Tamanho {PageSize}", 
            correlationId, conversationId, _logado.IdUtilizador, page, pageSize);

                // Verificar se usuário faz parte da conversa
                if (!await _unitOfWork.Conversations.IsUserInConversationAsync(conversationId, _logado.IdUtilizador))
                {
                            
                _logger.LogWarning("⚠️ [MESSAGE-{CorrelationId}] Usuário {UserId} não tem acesso à conversa {ConversationId}", 
                correlationId, _logado.IdUtilizador, conversationId);
    
                    // ✅ CORREÇÃO: Usar StatusCode ao invés de Forbid com mensagem
                    return StatusCode(403, new { Error = "Usuário não tem acesso a esta conversa" });
                }

                var messages = await _unitOfWork.Messages.GetMessagesByConversationIdAsync(conversationId);
                var pagedMessages = messages.Skip((page - 1) * pageSize).Take(pageSize);

                return Ok(pagedMessages.Select(m => new
                {
                    m.Id,
                    m.SenderId,
                    m.ReceiverId,
                    m.Content,
                    m.Timestamp,
                    m.IsRead,
                    m.MessageType,
                    m.AttachmentUrl
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao obter mensagens da conversa: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao obter mensagens", Details = ex.Message });
            }
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetUserConversations()
        {
            try
            {
                var conversations = await _unitOfWork.Conversations.GetUserConversationsAsync(_logado.IdUtilizador);
                
                return Ok(conversations.Select(c => new
                {
                    c.Id,
                    c.ConversationId,
                    c.UnreadCount,
                    c.UpdatedAt,
                    LastMessage = c.LastMessage != null ? new
                    {
                        c.LastMessage.Id,
                        c.LastMessage.Content,
                        c.LastMessage.Timestamp,
                        c.LastMessage.SenderId
                    } : null,
                    Participants = c.Participants.Select(p => new
                    {
                        p.UserId,
                        p.User.UserName,
                        p.JoinedAt
                    })
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao obter conversas: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao obter conversas", Details = ex.Message });
            }
        }
    [HttpPut("{messageId}/read")]
public async Task<IActionResult> MarkMessageAsRead(int messageId)
{
    try
    {
        var userId = _logado.IdUtilizador;
        var correlationId = Guid.NewGuid().ToString("N")[..8];
        
        _logger.LogInformation("📖 [MESSAGE-{CorrelationId}] Marcando mensagem {MessageId} como lida para usuário {UserId}", 
            correlationId, messageId, userId);

        // Buscar a mensagem
        var message = await _unitOfWork.Messages.Get(messageId);
        if (message == null)
        {
            return NotFound(new { Error = "Mensagem não encontrada" });
        }

        // Verificar se o usuário é o destinatário da mensagem
        if (message.ReceiverId != userId)
        {
            // ✅ CORREÇÃO: Usar StatusCode ao invés de Forbid com mensagem
            return StatusCode(403, new { Error = "Você não tem permissão para marcar esta mensagem como lida" });
        }

        // Verificar se a mensagem já foi lida
        if (message.IsRead)
        {
            return Ok(new { 
                Success = true, 
                Message = "Mensagem já estava marcada como lida",
                MessageId = messageId,
                IsRead = true
            });
        }

        // Marcar como lida
        message.MarkAsRead();
        await _unitOfWork.Messages.Update(message);

        // Atualizar contador da conversa
        var conversation = await _unitOfWork.Conversations.GetByConversationIdAsync(message.ConversationId);
        if (conversation != null && conversation.UnreadCount > 0)
        {
            var remainingUnread = await _unitOfWork.Messages
                .GetUnreadCountByConversationIdAsync(message.ConversationId, userId);
            
            if (remainingUnread == 0)
            {
                conversation.ResetUnreadCount();
                await _unitOfWork.Conversations.Update(conversation);
            }
        }

        await _unitOfWork.Save();

        _logger.LogInformation("✅ [MESSAGE-{CorrelationId}] Mensagem {MessageId} marcada como lida", 
            correlationId, messageId);

        return Ok(new { 
            Success = true, 
            Message = "Mensagem marcada como lida com sucesso",
            MessageId = messageId,
            IsRead = true,
            Timestamp = message.Timestamp
        });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "💥 Erro ao marcar mensagem como lida: {Message}", ex.Message);
        return BadRequest(new { Error = "Erro ao marcar mensagem como lida", Details = ex.Message });
    }
}

/// <summary>
/// Marcar múltiplas mensagens como lidas
/// </summary>






        [HttpPut("conversation/{conversationId}/read")]
        public async Task<IActionResult> MarkConversationAsRead(string conversationId)
        {
            try
            {
                await _unitOfWork.Messages.MarkMessagesAsReadAsync(conversationId, _logado.IdUtilizador);
                
                var conversation = await _unitOfWork.Conversations.GetByConversationIdAsync(conversationId);
                conversation?.ResetUnreadCount();
                
                if (conversation != null)
                {
                    await _unitOfWork.Conversations.Update(conversation);
                }
                
                await _unitOfWork.Save();

                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao marcar conversa como lida: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao marcar como lida", Details = ex.Message });
            }
        }

        /// <summary>
        /// Iniciar uma nova conversa com um usuário específico
        /// </summary>
        [HttpPost("conversations/start")]
        public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
        {
            try
            {
                var currentUserId = _logado.IdUtilizador;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new { Error = "Usuário não autenticado" });
                }

                var correlationId = Guid.NewGuid().ToString("N")[..8];
                _logger.LogInformation("💬 [CONVERSATION-{CorrelationId}] Iniciando conversa entre {CurrentUserId} e {TargetUserId}", 
                    correlationId, currentUserId, request.UserId);

                // Validar se o usuário de destino existe
                if (string.IsNullOrEmpty(request.UserId))
                {
                    return BadRequest(new { Error = "ID do usuário destinatário é obrigatório" });
                }

                // Verificar se não está tentando conversar consigo mesmo
                if (currentUserId == request.UserId)
                {
                    return BadRequest(new { Error = "Não é possível iniciar uma conversa consigo mesmo" });
                }

                var participantIds = new List<string> { currentUserId, request.UserId };

                // Verificar se já existe uma conversa entre esses usuários
                var existingConversationId = await _unitOfWork.Conversations
                    .GetExistingConversationIdAsync(participantIds);

                if (!string.IsNullOrEmpty(existingConversationId))
                {
                    _logger.LogInformation("💬 [CONVERSATION-{CorrelationId}] Conversa existente encontrada: {ConversationId}", 
                        correlationId, existingConversationId);

                    var existingConversation = await _unitOfWork.Conversations
                        .GetByConversationIdAsync(existingConversationId);

                    // Obter participantes da conversa
                    var participants = await _unitOfWork.ConversationParticipants
                        .GetConversationParticipantsAsync(existingConversationId);

                    // Obter última mensagem
                    var lastMessage = await _unitOfWork.Messages
                        .GetLastMessageByConversationIdAsync(existingConversationId);

                    // Obter contador de não lidas
                    var unreadCount = await _unitOfWork.Messages
                        .GetUnreadCountByConversationIdAsync(existingConversationId, currentUserId);

                    return Ok(new
                    {
                        Id = existingConversation.Id,
                        ConversationId = existingConversationId,
                        IsNew = false,
                        Participants = participants.Select(p => new
                        {
                            UserId = p.UserId,
                            JoinedAt = p.JoinedAt,
                            IsActive = p.IsActive
                        }),
                        LastMessage = lastMessage != null ? new
                        {
                            Id = lastMessage.Id,
                            Content = lastMessage.Content,
                            SenderId = lastMessage.SenderId,
                            Timestamp = lastMessage.Timestamp,
                            MessageType = lastMessage.MessageType.ToString()
                        } : null,
                        UnreadCount = unreadCount,
                        UpdatedAt = existingConversation.UpdatedAt,
                        CreatedAt = existingConversation.DateCreated,
                        Success = true,
                        Message = "Conversa existente retornada"
                    });
                }

                // Gerar ID único para a nova conversa
                var newConversationId = await _unitOfWork.Conversations
                    .GenerateConversationIdAsync(participantIds);

                // Criar nova conversa
                var conversation = new Hotel.Domain.Entities.Conversation(newConversationId, participantIds);
                await _unitOfWork.Conversations.Add(conversation);

                // Adicionar participantes à conversa
                foreach (var participantId in participantIds)
                {
                    await _unitOfWork.ConversationParticipants
                        .AddParticipantAsync(newConversationId, participantId);
                }

                // Se foi fornecida uma mensagem inicial, enviá-la
                if (!string.IsNullOrEmpty(request.InitialMessage))
                {
                    var initialMessage = new Hotel.Domain.Entities.Message(
                        currentUserId,
                        request.UserId,
                        request.InitialMessage,
                        Hotel.Domain.Entities.MessageType.Text,
                        newConversationId
                    );

                    await _unitOfWork.Messages.Add(initialMessage);
                    conversation.UpdateLastMessage(initialMessage);
                    
                    // Criar notificação para o destinatário
                    var notification = new Hotel.Domain.Entities.MessageNotification(
                        initialMessage.Id, currentUserId, request.UserId);
                    await _unitOfWork.MessageNotifications.Add(notification);
                }

                await _unitOfWork.Save();

                // Obter participantes criados
                var newParticipants = await _unitOfWork.ConversationParticipants
                    .GetConversationParticipantsAsync(newConversationId);

                _logger.LogInformation("✅ [CONVERSATION-{CorrelationId}] Nova conversa criada: {ConversationId}", 
                    correlationId, newConversationId);

                return CreatedAtAction(nameof(GetConversationMessages), new { conversationId = newConversationId }, new
                {
                    Id = conversation.Id,
                    ConversationId = newConversationId,
                    IsNew = true,
                    Participants = newParticipants.Select(p => new
                    {
                        UserId = p.UserId,
                        JoinedAt = p.JoinedAt,
                        IsActive = p.IsActive
                    }),
                    LastMessage = (object)null,
                    UnreadCount = 0,
                    UpdatedAt = conversation.UpdatedAt,
                    CreatedAt = conversation.DateCreated,
                    Success = true,
                    Message = "Nova conversa criada com sucesso"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao iniciar conversa: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao iniciar conversa", Details = ex.Message });
            }
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadMessages()
        {
            try
            {
                var unreadMessages = await _unitOfWork.Messages.GetUnreadMessagesAsync(_logado.IdUtilizador);
                
                return Ok(unreadMessages.Select(m => new
                {
                    m.Id,
                    m.SenderId,
                    m.Content,
                    m.Timestamp,
                    m.ConversationId,
                    m.MessageType
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao obter mensagens não lidas: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao obter mensagens não lidas", Details = ex.Message });
            }
        }
    }
}