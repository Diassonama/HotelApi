using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common;
using Hotel.Application.Contracts;
using Hotel.Application.Interfaces.Services;
using MediatR;
using Serilog;

namespace Hotel.Application.User.Commands.RecoverPassword
{
    public class RecoverPasswordCommand : IRequest<string>
    {
        public string Username { get; set; }
        public class RecoverPasswordCommandHandler : IRequestHandler<RecoverPasswordCommand, string>
        {
            private readonly IEmailService _emailService;
            private readonly ISMSService _smsService;
            private readonly IAuthService _authService;
            private readonly ILogger _logger;

            public RecoverPasswordCommandHandler(IEmailService emailService, ISMSService smsService, IAuthService authService, ILogger logger)
            {
                _emailService = emailService;
                _smsService = smsService;
                _authService = authService;
                _logger = logger;
            }

            public async Task<string> Handle(RecoverPasswordCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    // ✅ CORREÇÃO: Usar await para chamada assíncrona
                    var user = await _authService.FindUserByEmailAsync(request.Username);
                    
                    if (user == null)
                    {
                        _logger.Warning($"Usuário não encontrado para recuperação de senha: {request.Username}");
                        return "Se o email estiver registrado, você receberá as instruções de recuperação.";
                    }

                    var userId = user.Id.ToString();

                    // Recuperar usando SMS
                    var token = await _authService.GeneratePhoneChangeTokenAsyncUser(request.Username, "922285032");
                    await _smsService.SendConfirmationSMS("922285032", token);

                    // Recuperar usando Email
                    var emailToken = await _authService.GeneratePasswordResetTokenAsync(request.Username);
                    if (emailToken != null)
                    {
                        await _emailService.SendEmailBeforeChangePassword(request.Username, emailToken);
                    }
                    
                    return "Instruções de recuperação enviadas com sucesso!";
                }
                catch (Exception ex)
                {
                    _logger.Error($"Erro ao recuperar a password. Exception: {ex}");
                    return "Erro interno do servidor. Tente novamente mais tarde.";
                }
            }
        }

    }
}