using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common;
using Hotel.Application.Contracts;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using MediatR;

namespace Hotel.Application.User.Commands.LogoutUser
{
    public class LogoutUserCommand: IRequest<BaseCommandResponse>
    {
        public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, BaseCommandResponse>
        {
            private readonly UsuarioLogado _logado;
            private readonly IAuthService _authService;

            public LogoutUserCommandHandler(UsuarioLogado logado, IAuthService authService)
            {
                _logado = logado;
                _authService = authService;
            }

            public async Task<BaseCommandResponse> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
            {
                var result = await _authService.SignOutAsync(_logado.IdUtilizador);
                return result;
            }
        }

    }
}