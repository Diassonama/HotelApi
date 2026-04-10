using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.TipoHospedagem.Queries
{
    public class GetTipoHospedagemQuery: IRequest<BaseCommandResponse>
    {
        public class GetTipoHospedagemQueryHandler : IRequestHandler<GetTipoHospedagemQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetTipoHospedagemQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetTipoHospedagemQuery request, CancellationToken cancellationToken)
            {
               var resposta = new BaseCommandResponse();
               var data = await _unitOfWork.TipoHospedagem.GetAllAsync();

               if (data==null){
                   resposta.Success = false;
                   resposta.Message = "Não foi possível encontrar o tipo de hospedagem";
                   return resposta;
               }

               resposta.Success = true; 
               resposta.Data = data;
               resposta.Message = "Dados carregado com sucesso";
               return resposta;

            }
        }
    }
}