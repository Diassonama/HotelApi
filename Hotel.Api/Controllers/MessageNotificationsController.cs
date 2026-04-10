using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessageNotificationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MessageNotificationsController> _logger;

        public MessageNotificationsController(IUnitOfWork unitOfWork, ILogger<MessageNotificationsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Obter todas as notificações do usuário
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Error = "Usuário não autenticado" });
                }

                var correlationId = Guid.NewGuid().ToString("N")[..8];
                _logger.LogInformation("🔔 [NOTIFICATION-{CorrelationId}] Obtendo notificações para usuário {UserId}", 
                    correlationId, userId);

                // ✅ CORREÇÃO: Usar método que existe no repository
                var notifications = await _unitOfWork.MessageNotifications.GetUserNotificationsAsync(userId);
                
                if (notifications == null)
                {
                    _logger.LogWarning("⚠️ [NOTIFICATION-{CorrelationId}] Nenhuma notificação encontrada para usuário {UserId}", 
                        correlationId, userId);
                    return Ok(new List<object>());
                }

                var result = notifications.Select(n => new
                {
                    Id = n.Id,
                    MessageId = n.MessageId,
                    SenderId = n.SenderId,
                    ReceiverId = n.ReceiverId,
                    IsNew = n.IsNew,
                    IsRead = n.IsRead,
                    Timestamp = n.Timestamp,
                    SenderName = GetUserName(n.SenderId),
                    MessagePreview = GetMessagePreview(n.MessageId)
                }).ToList();

                _logger.LogInformation("✅ [NOTIFICATION-{CorrelationId}] {Count} notificações encontradas", 
                    correlationId, result.Count);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao obter notificações: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao obter notificações", Details = ex.Message });
            }
        }

        /// <summary>
        /// Obter apenas notificações não lidas
        /// </summary>
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Error = "Usuário não autenticado" });
                }

                var correlationId = Guid.NewGuid().ToString("N")[..8];
                _logger.LogInformation("🔔 [NOTIFICATION-{CorrelationId}] Obtendo notificações não lidas para usuário {UserId}", 
                    correlationId, userId);

                // ✅ CORREÇÃO: Usar método que existe no repository
                var notifications = await _unitOfWork.MessageNotifications.GetUnreadNotificationsAsync(userId);
                
                if (notifications == null)
                {
                    _logger.LogWarning("⚠️ [NOTIFICATION-{CorrelationId}] Nenhuma notificação não lida encontrada para usuário {UserId}", 
                        correlationId, userId);
                    return Ok(new List<object>());
                }

                var result = notifications.Select(n => new
                {
                    Id = n.Id,
                    MessageId = n.MessageId,
                    SenderId = n.SenderId,
                    IsNew = n.IsNew,
                    Timestamp = n.Timestamp,
                    SenderName = GetUserName(n.SenderId),
                    MessagePreview = GetMessagePreview(n.MessageId)
                }).ToList();

                _logger.LogInformation("✅ [NOTIFICATION-{CorrelationId}] {Count} notificações não lidas encontradas", 
                    correlationId, result.Count);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao obter notificações não lidas: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao obter notificações não lidas", Details = ex.Message });
            }
        }

        /// <summary>
        /// Obter contador de notificações não lidas
        /// </summary>
        [HttpGet("unread/count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Error = "Usuário não autenticado" });
                }

                // ✅ CORREÇÃO: Usar método que existe no repository
                var count = await _unitOfWork.MessageNotifications.GetUnreadNotificationCountAsync(userId);
                
                return Ok(new { UnreadCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao obter contador de notificações: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao obter contador", Details = ex.Message });
            }
        }

        /// <summary>
        /// Marcar notificação como lida
        /// </summary>
        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var correlationId = Guid.NewGuid().ToString("N")[..8];
                
                _logger.LogInformation("🔔 [NOTIFICATION-{CorrelationId}] Marcando notificação {NotificationId} como lida", 
                    correlationId, notificationId);

                // ✅ CORREÇÃO: Usar método correto do repository
                var notification = await _unitOfWork.MessageNotifications.GetNotificationByIdAsync(notificationId);
                if (notification == null)
                {
                    return NotFound(new { Error = "Notificação não encontrada" });
                }

                if (notification.ReceiverId != userId)
                {
                    // ✅ CORREÇÃO: Usar StatusCode ao invés de Forbid
                    return StatusCode(403, new { Error = "Você não tem permissão para acessar esta notificação" });
                }

                // ✅ CORREÇÃO: Marcar como lida usando método da entidade
                notification.MarkAsRead();
                await _unitOfWork.MessageNotifications.Update(notification);
                await _unitOfWork.Save();

                _logger.LogInformation("✅ [NOTIFICATION-{CorrelationId}] Notificação marcada como lida", correlationId);

                return Ok(new { Success = true, Message = "Notificação marcada como lida" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao marcar notificação como lida: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao marcar como lida", Details = ex.Message });
            }
        }

        /// <summary>
        /// Marcar todas as notificações como lidas
        /// </summary>
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = GetCurrentUserId();
                var correlationId = Guid.NewGuid().ToString("N")[..8];
                
                _logger.LogInformation("🔔 [NOTIFICATION-{CorrelationId}] Marcando todas as notificações como lidas para usuário {UserId}", 
                    correlationId, userId);

                // ✅ CORREÇÃO: Implementar marcação manual
                var unreadNotifications = await _unitOfWork.MessageNotifications.GetUnreadNotificationsAsync(userId);
                
                if (unreadNotifications != null && unreadNotifications.Any())
                {
                    foreach (var notification in unreadNotifications)
                    {
                        notification.MarkAsRead();
                        await _unitOfWork.MessageNotifications.Update(notification);
                    }
                    
                    await _unitOfWork.Save();
                    
                    _logger.LogInformation("✅ [NOTIFICATION-{CorrelationId}] {Count} notificações marcadas como lidas", 
                        correlationId, unreadNotifications.Count());
                }

                return Ok(new { Success = true, Message = "Todas as notificações marcadas como lidas" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao marcar todas as notificações como lidas: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao marcar todas como lidas", Details = ex.Message });
            }
        }

        /// <summary>
        /// Obter notificações recentes
        /// </summary>
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentNotifications([FromQuery] int count = 10)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { Error = "Usuário não autenticado" });
                }

                // ✅ CORREÇÃO: Usar método que existe no repository
                var notifications = await _unitOfWork.MessageNotifications.GetRecentNotificationsAsync(userId, count);
                
                if (notifications == null)
                {
                    return Ok(new List<object>());
                }

                var result = notifications.Select(n => new
                {
                    Id = n.Id,
                    MessageId = n.MessageId,
                    SenderId = n.SenderId,
                    IsNew = n.IsNew,
                    IsRead = n.IsRead,
                    Timestamp = n.Timestamp,
                    SenderName = GetUserName(n.SenderId),
                    MessagePreview = GetMessagePreview(n.MessageId)
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao obter notificações recentes: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao obter notificações recentes", Details = ex.Message });
            }
        }

        /// <summary>
        /// Deletar notificação
        /// </summary>
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var correlationId = Guid.NewGuid().ToString("N")[..8];
                
                _logger.LogInformation("🔔 [NOTIFICATION-{CorrelationId}] Deletando notificação {NotificationId}", 
                    correlationId, notificationId);

                // ✅ CORREÇÃO: Usar método correto do repository
                var notification = await _unitOfWork.MessageNotifications.GetNotificationByIdAsync(notificationId);
                if (notification == null)
                {
                    return NotFound(new { Error = "Notificação não encontrada" });
                }

                if (notification.ReceiverId != userId)
                {
                    // ✅ CORREÇÃO: Usar StatusCode ao invés de Forbid
                    return StatusCode(403, new { Error = "Você não tem permissão para deletar esta notificação" });
                }

                // ✅ CORREÇÃO: Usar soft delete
                await _unitOfWork.MessageNotifications.Delete(notification);
                await _unitOfWork.Save();

                _logger.LogInformation("✅ [NOTIFICATION-{CorrelationId}] Notificação deletada", correlationId);

                return Ok(new { Success = true, Message = "Notificação deletada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao deletar notificação: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao deletar notificação", Details = ex.Message });
            }
        }

        /// <summary>
        /// Deletar todas as notificações do usuário
        /// </summary>
        [HttpDelete("all")]
        public async Task<IActionResult> DeleteAllNotifications()
        {
            try
            {
                var userId = GetCurrentUserId();
                var correlationId = Guid.NewGuid().ToString("N")[..8];
                
                _logger.LogInformation("🔔 [NOTIFICATION-{CorrelationId}] Deletando todas as notificações para usuário {UserId}", 
                    correlationId, userId);

                // ✅ CORREÇÃO: Implementar deleção manual
                var userNotifications = await _unitOfWork.MessageNotifications.GetUserNotificationsAsync(userId);
                
                if (userNotifications != null && userNotifications.Any())
                {
                    foreach (var notification in userNotifications)
                    {
                        await _unitOfWork.MessageNotifications.Delete(notification);
                    }
                    
                    await _unitOfWork.Save();
                    
                    _logger.LogInformation("✅ [NOTIFICATION-{CorrelationId}] {Count} notificações deletadas", 
                        correlationId, userNotifications.Count());
                }

                return Ok(new { Success = true, Message = "Todas as notificações deletadas" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Erro ao deletar todas as notificações: {Message}", ex.Message);
                return BadRequest(new { Error = "Erro ao deletar todas as notificações", Details = ex.Message });
            }
        }

        #region Helper Methods

        private string GetCurrentUserId()
        {
            return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private string GetUserName(string userId)
        {
            try
            {
                // ✅ TODO: Implementar busca de nome do usuário no cache ou banco
                // Por enquanto retornar o ID truncado
                return string.IsNullOrEmpty(userId) ? "Usuário Desconhecido" : $"User_{userId.Substring(0, 8)}";
            }
            catch
            {
                return "Usuário Desconhecido";
            }
        }

        private string GetMessagePreview(int messageId)
        {
            try
            {
                // ✅ TODO: Implementar busca de preview da mensagem
                // Por enquanto retornar placeholder
                return "Nova mensagem...";
            }
            catch
            {
                return "Nova mensagem...";
            }
        }

        #endregion
    }
}