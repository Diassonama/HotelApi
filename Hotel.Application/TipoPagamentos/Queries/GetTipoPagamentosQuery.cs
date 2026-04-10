using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Application.TipoPagamentos.Queries
{
    public class GetTipoPagamentosQuery:IRequest<BaseCommandResponse>
    {
        public class GetTipoPagamentosQueryHandler : IRequestHandler<GetTipoPagamentosQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetTipoPagamentosQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetTipoPagamentosQuery request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var dados = await _unitOfWork.TipoPagamentos.GetAll().ToListAsync();

                if(dados ==null)
                {
                    resposta.Success = false;
                    resposta.Message = "Não foi possível encontrar os tipos de pagamento";
                    return resposta;
                }

                resposta.Success = true;
                resposta.Data = dados;
                resposta.Message ="Dados carregados com sucessos";
                return resposta;
                
            }
        }
    }
}