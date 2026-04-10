using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Hospedagem.Queries
{
    public class GetHospedagemQuery: IRequest<BaseCommandResponse>
    {
        public class GetHospedagemQueryHandler : IRequestHandler<GetHospedagemQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork  _unitOfWork;

            public GetHospedagemQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetHospedagemQuery request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();
                var hospedagem = await _unitOfWork.Hospedagem.GetHospedagemAsync();

                if(hospedagem is null)
                {
                    response.Message = "Dado(s) não encontrado";
                    response.Success = false;
                    return response;
                }

                response.Data = hospedagem;
                response.Success = true;
                response.Message = "Dado(s) carregado com sucesso";
                await _unitOfWork.Apartamento.AtualizarSituacaoApartamentosAsync();
                return   await Task.FromResult(response);
            }
        }

    }
}