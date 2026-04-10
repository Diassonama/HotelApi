using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Hotel.Application.Common;
using Hotel.Application.Contracts;
using Hotel.Application.Services;
using MediatR;

namespace Hotel.Application.User.Commands.changePassword
{
    public class ChangePasswordCommad : IRequest<Result>
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

        public class ChangePasswordCommadHandler : IRequestHandler<ChangePasswordCommad, Result>
        {
            private readonly IAuthService _authService;
            private readonly UsuarioLogado _logado;
            private readonly ILogger _logger;

            public ChangePasswordCommadHandler(IAuthService authService, UsuarioLogado logado, ILogger logger)
            {
                _authService = authService;
                _logado = logado;
                _logger = logger;
            }
            public async Task<Result> Handle(ChangePasswordCommad request, CancellationToken cancellationToken)
            {
                try
                {
                    var result = await _authService.ChangeUserPasswordAsync(_logado.IdUtilizador, request.CurrentPassword, request.NewPassword);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.Error("Erro ao alterar a palavra passe", ex, request);
                    throw;
                }
            }
        }
    }
}