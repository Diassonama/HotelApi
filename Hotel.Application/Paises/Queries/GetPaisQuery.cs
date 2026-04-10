using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Paises.Queries
{
    public class GetPaisQuery: IRequest<BaseCommandResponse>
    {
        public class GetPaisQueryHandler : IRequestHandler<GetPaisQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetPaisQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetPaisQuery request, CancellationToken cancellationToken)
            {
               var resposta = new BaseCommandResponse();

               var paises = await _unitOfWork.Paises.GetAllAsync();

               if(paises ==null){
                resposta.Success = false;
                resposta.Message = "Dados não encontrados";
                return resposta;
               }

               resposta.Success = true;
               resposta.Message = "Dados carregados com sucesso";
               resposta.Data = paises;

               return resposta;
            }
            
        }
    }
}