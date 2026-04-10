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
    public class GetCaixaByIdQuery: IRequest<BaseCommandResponse>
    {
        public int Id { get; set; }
        public class GetCaixaByIdQueryHandler : IRequestHandler<GetCaixaByIdQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly UsuarioLogado _usuarioLogado;

            public GetCaixaByIdQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetCaixaByIdQuery request, CancellationToken cancellationToken)
            {
                 var response = new BaseCommandResponse();
                var hospedagem = await _unitOfWork.caixa.GetByIdAsync(request.Id);

                if(hospedagem is null)
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