using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Hotel.Application.Contracts;
using Hotel.Application.Services;
using MediatR;
using Serilog;

namespace Hotel.Application.User.Commands.ConfirmEmail
{
    public class ConfirmeEmailCommand : IRequest<bool>
    {
        public string Token { get; set; }
        public string Email { get; set; }

        public class ConfirmeEmailCommandHandler : IRequestHandler<ConfirmeEmailCommand, bool>
        {
           // private readonly UsuarioLogado _usuarioLogado;
            private readonly IAuthService _authService;
            private readonly ILogger logger;


            public ConfirmeEmailCommandHandler(IAuthService authService, ILogger logger)
            {
                _authService = authService;
                this.logger = logger;
            }

            public async Task<bool> Handle(ConfirmeEmailCommand request, CancellationToken cancellationToken)
            {
                var user = await _authService.FindUserByEmailAsync(request.Email);
                if(user == null)  throw new Exception("Email de confirmação incorreto");

                var sucess = await _authService.ConfirmEmailAsync( request.Token, request.Email);

                if(sucess)
                {
                    //TODO: enviar email de confirmação
                    logger.Information($"Email confirmado com sucesso {request.Email}");
                 //   var user = _authService.FindUserByIdAsync(userId);
                    
                }

                return sucess;
            }
        }

    }
}