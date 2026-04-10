using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Caixa.Queries
{
    public class GetCaixaQuery: IRequest<BaseCommandResponse>
    {
        public class GetCaixaQueryHandler : IRequestHandler<GetCaixaQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly UsuarioLogado _usuarioLogado;

            public GetCaixaQueryHandler(IUnitOfWork unitOfWork, UsuarioLogado usuarioLogado)
            {
                _unitOfWork = unitOfWork;
                _usuarioLogado = usuarioLogado;
            }

            public async Task<BaseCommandResponse> Handle(GetCaixaQuery request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();
                var hospedagem = await _unitOfWork.caixa.GetAllAsync(_usuarioLogado.UsuarioId, _usuarioLogado.perfil);

                if(hospedagem == null)
                {
                    response.Message = "Dado(s) não encontrado";
                    response.Success = false;
                    return response;
                }

                response.Data = hospedagem;
                response.Success = true;
                response.Message = "Dado(s) carregado com sucesso";
                return   await Task.FromResult(response);
            }
        }
    }
}