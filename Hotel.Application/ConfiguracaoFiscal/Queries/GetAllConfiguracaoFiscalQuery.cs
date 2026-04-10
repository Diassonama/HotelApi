using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.ConfiguracaoFiscal.Queries
{
    public class GetAllConfiguracaoFiscalQuery: IRequest<BaseCommandResponse>
    {
        public class GetAllConfiguracaoFiscalQueryHandler : IRequestHandler<GetAllConfiguracaoFiscalQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly ISeriesRepository _series ;

            public GetAllConfiguracaoFiscalQueryHandler(IUnitOfWork unitOfWork, ISeriesRepository series)
            {
                _unitOfWork = unitOfWork;
                _series = series;
            }

            public async Task<BaseCommandResponse> Handle(GetAllConfiguracaoFiscalQuery request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();

                var configuracaoFiscal = await _unitOfWork.Param.GetAllAsync();
                                          _series.CriarSerieAsync();


                if (configuracaoFiscal == null)  
                {
                    resposta.Success = false;
                    resposta.Message = "configuração fiscal não encontrado";
                }
                
                    resposta.Success = true;
                    resposta.Message = "Configuração fiscal carregado com sucesso";
                    resposta.Data = configuracaoFiscal;
                    return resposta;
                
            }
        }
    }
}