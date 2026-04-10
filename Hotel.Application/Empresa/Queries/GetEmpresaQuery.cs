using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Empresa.Queries
{
    public class GetEmpresaQuery: IRequest<BaseCommandResponse>
    {
        public class GetEmpresaQueryHandler : IRequestHandler<GetEmpresaQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetEmpresaQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async  Task<BaseCommandResponse> Handle(GetEmpresaQuery request, CancellationToken cancellationToken)
            {
               var resposta = new BaseCommandResponse();
               var data = await _unitOfWork.Empresa.GetAllAsync();

               if (data==null){
                   resposta.Success = false;
                   resposta.Message = "Não foi possível encontrar os dados";
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