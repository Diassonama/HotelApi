using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.MenuRole.Queries
{
    public class GetMenuRoleQuery:IRequest<BaseCommandResponse>
    {
        public class GetMenuRoleQueryHandler : IRequestHandler<GetMenuRoleQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetMenuRoleQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetMenuRoleQuery request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var dados = await _unitOfWork.MenuRole.GetMenuAsync();

                if(dados ==null){
                    resposta.Success = false;
                    resposta.Message = "Menu role não existe";
                    return resposta;
                }

                resposta.Success = true;    
                resposta.Message = "Menu role encontrado";
                resposta.Data = dados;
                return resposta;
            }
        }
    }
}