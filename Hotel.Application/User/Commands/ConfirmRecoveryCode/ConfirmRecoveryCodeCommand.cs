using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common;
using Hotel.Application.Contracts;
using Hotel.Application.Interfaces.Services;
using Hotel.Application.Responses;
using MediatR;
using Serilog;
// Add the correct namespace for BaseCommandResponse if it's different
// using Hotel.Application.Responses; // Ensure this is the correct namespace

namespace Hotel.Application.User.Commands.ConfirmRecoveryCode
{
    public class ConfirmRecoveryCodeCommand : IRequest<BaseCommandResponse>
    {
        [Required(ErrorMessage = "Username é obrigatório")]
        [EmailAddress(ErrorMessage = "Username deve ser um email válido")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Código de confirmação é obrigatório")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Código deve ter entre 4 e 10 caracteres")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Nova senha deve ter pelo menos 6 caracteres")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
        [Compare("NewPassword", ErrorMessage = "Confirmação de senha não confere")]
        public string ConfirmPassword { get; set; }

        public class ConfirmRecoveryCodeCommandHandler : IRequestHandler<ConfirmRecoveryCodeCommand, BaseCommandResponse>
        {
            private readonly IAuthService _authService;
            private readonly ILogger _logger;

            public ConfirmRecoveryCodeCommandHandler(IAuthService authService, ILogger logger)
            {
                _authService = authService;
                _logger = logger;
            }

            public async Task<BaseCommandResponse> Handle(ConfirmRecoveryCodeCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();

                try
                {
                    // Validar se o usuário existe
                    var user = await _authService.FindUserByEmailAsync(request.Username);
                    if (user == null)
                    {
                        response.Success = false;
                        response.Message = "❌ Usuário não encontrado";
                        response.Errors = new List<string> { "Email não encontrado no sistema" };
                        _logger.Warning($"Tentativa de confirmação de código para usuário inexistente: {request.Username}");
                        return response;
                    }

                    // Verificar se o código está válido
                    var isCodeValid = await _authService.VerifyPasswordResetTokenAsync(request.Username, request.Code);
                    if (!isCodeValid)
                    {
                        response.Success = false;
                        response.Message = "❌ Código inválido ou expirado";
                        response.Errors = new List<string> { "O código de confirmação está inválido ou expirou. Solicite um novo código." };
                        _logger.Warning($"Código inválido para recuperação de senha: {request.Username}");
                        return response;
                    }

                    // Resetar a senha usando o token válido
                    var resetResult = await _authService.ResetPasswordAsync(request.Username, request.Code, request.NewPassword);
                    if (!resetResult)
                    {
                        response.Success = false;
                        response.Message = "❌ Erro ao redefinir a senha";
                        response.Errors = new List<string> { "Token inválido ou expirado, ou a nova senha não atende aos critérios de segurança" };
                        _logger.Error($"Erro ao redefinir senha para usuário {request.Username}");
                        return response;
                    }

                    response.Success = true;
                    response.Message = "✅ Senha redefinida com sucesso! Você já pode fazer login com a nova senha.";
                    _logger.Information($"Senha redefinida com sucesso para usuário: {request.Username}");

                    return response;
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = "❌ Erro interno do servidor";
                    response.Errors = new List<string> { "Erro interno. Tente novamente mais tarde." };
                    _logger.Error($"Erro ao confirmar código de recuperação para {request.Username}. Exception: {ex}");
                    return response;
                }
            }
        }
    }
}
