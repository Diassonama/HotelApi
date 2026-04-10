using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.MotivoViagem.Queries
{
    public class GetMotivoViagemQuery:IRequest<BaseCommandResponse>
    {
        public class GetMotivoViagemQueryHandler : IRequestHandler<GetMotivoViagemQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetMotivoViagemQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetMotivoViagemQuery request, CancellationToken cancellationToken)
            {
               var resposta = new BaseCommandResponse();
               var data = await _unitOfWork.MotivoViagem.GetAllAsync();

               if (data==null){
                   resposta.Success = false;
                   resposta.Message = "Não foi possível encontrar o motivo de viagem";
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