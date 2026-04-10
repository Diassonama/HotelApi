using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Empresa.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Identity.Client;

namespace Hotel.Application.Empresa.Queries
{
    public class GetEmpresaByIdQuery: IRequest<BaseCommandResponse>
    {
        public int Id { get; set; }
        public class GetEmpresaByIdQueryHandler : IRequestHandler<GetEmpresaByIdQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetEmpresaByIdQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetEmpresaByIdQuery request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();

                var empresa = await _unitOfWork.Empresa.Get(request.Id);
                if(empresa==null){
                    resposta.Message="Empresa não encontrada";
                    resposta.Success= false;
                    return resposta;
                }

                resposta.Message ="Empresa carregado com sucesso";
                resposta.Data = empresa;
                resposta.Success = true;

                return resposta;

            }
        }

    }
}