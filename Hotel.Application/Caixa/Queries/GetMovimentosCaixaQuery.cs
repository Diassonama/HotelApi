using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Services;
using Hotel.Domain.Identity;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Hotel.Application.Caixa.Queries
{
    public class GetMovimentosCaixaQuery: IRequest<object>
    {
        public class GetMovimentosCaixaQueryHandler : IRequestHandler<GetMovimentosCaixaQuery, object>
        {
            private readonly IUnitOfWork _unitOfWork;
             public readonly UserManager<ApplicationUser> _userManager;
            private readonly UsuarioLogado _usuarioLogado;

            public GetMovimentosCaixaQueryHandler(IUnitOfWork unitOfWork, UsuarioLogado usuarioLogado, UserManager<ApplicationUser> userManager)
            {
                _unitOfWork = unitOfWork;
                _usuarioLogado = usuarioLogado;
                _userManager = userManager;
            }

            public async Task<object> Handle(GetMovimentosCaixaQuery request, CancellationToken cancellationToken)
            {
                Log.Information("Iniciando manipulação de GetMovimentosCaixaQuery", _usuarioLogado.UsuarioId);
                // Aqui você pode adicionar lógica adicional, como filtragem baseada no usuário logado
                Log.Information("Recuperando movimentos do caixa para o usuário {UsuarioId}", _usuarioLogado.UsuarioId  ?? "Anônimo"        );
                // Aqui você pode adicionar lógica adicional, como filtragem baseada no usuário logado
                Log.Information("Recuperando movimentos do caixa para o usuário {UsuarioId}", _usuarioLogado.UsuarioId  ?? "Anônimo"        );
                //pega o role do usuario logado
                var user = await _userManager.FindByIdAsync(_usuarioLogado.UsuarioId);
                var role = await _userManager.GetRolesAsync(user);
                Log.Information("Usuário {UsuarioId} tem o papel {Role}", _usuarioLogado.UsuarioId ?? "Anônimo", string.Join(", ", role));
              return await _unitOfWork.caixa.MovimentoDoCaixa(_usuarioLogado.UsuarioId, _usuarioLogado.perfil);
                //return await _unitOfWork.caixa.MovimentoDoCaixa();
            }
        }
    }
}