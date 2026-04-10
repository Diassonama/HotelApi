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
    public class GetCaixaByDateQuery: IRequest<BaseCommandResponse>
    {
        public DateTime data { get; set; }
        public class GetCaixaByDateQueryHandler : IRequestHandler<GetCaixaByDateQuery, BaseCommandResponse>
        {
            private IUnitOfWork _unitOfWork;
             private readonly UsuarioLogado _usuarioLogado;

            public GetCaixaByDateQueryHandler(IUnitOfWork unitOfWork, UsuarioLogado usuarioLogado)
            {
                _unitOfWork = unitOfWork;
                _usuarioLogado = usuarioLogado;
            }

            public async  Task<BaseCommandResponse> Handle(GetCaixaByDateQuery request, CancellationToken cancellationToken)
            {
                    var response = new BaseCommandResponse();
                var hospedagem = await _unitOfWork.caixa.GetByDateAsync(request.data, _usuarioLogado.IdUtilizador, _usuarioLogado.perfil);

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