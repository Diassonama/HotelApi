using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.ConfiguracaoFiscal.Queries
{
    public class GetAppConfigAllQuery: IRequest<BaseCommandResponse>
    {
        public class GetAppConfigAllQueryHandler: IRequestHandler<GetAppConfigAllQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetAppConfigAllQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
               
            }

            public async Task<BaseCommandResponse> Handle(GetAppConfigAllQuery request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();

                var appConfig = await _unitOfWork.AppConfig.GetAllAsync();
                 if(appConfig == null){
                    resposta.Success = false;
                    resposta.Message = "Não existe configuração fiscal cadastrada";
                }

                resposta.Success = true;
                resposta.Message = "Configuração fiscal carregado com sucesso";
                resposta.Data = appConfig;
                return resposta;
            }                       
        }
    }
}

