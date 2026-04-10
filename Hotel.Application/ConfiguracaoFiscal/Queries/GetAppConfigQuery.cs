using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.ConfiguracaoFiscal.Queries
{
    public class GetAppConfigQuery: IRequest<BaseCommandResponse>
    {
        public string key { get; set; }

        public class GetAppConfigQueryHandler: IRequestHandler<GetAppConfigQuery, BaseCommandResponse>{
            private readonly IUnitOfWork _unitOfWork;

            public GetAppConfigQueryHandler(IUnitOfWork unitOfWork){
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetAppConfigQuery request, CancellationToken cancellationToken){
                var resposta = new BaseCommandResponse();
                var appConfig = await _unitOfWork.AppConfig.GetByKeyAsync(request.key);

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