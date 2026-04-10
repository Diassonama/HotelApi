using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.ConfiguracaoFiscal.Queries
{
    public class GetConfiguracaoFiscalByIdQuery : IRequest<BaseCommandResponse>
    {
        public int Id { get; set; }
        public class GetConfiguracaoFiscalByIdQueryHandler : IRequestHandler<GetConfiguracaoFiscalByIdQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetConfiguracaoFiscalByIdQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetConfiguracaoFiscalByIdQuery request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var configuracaoFiscal = await _unitOfWork.Param.Get(request.Id);

                if (configuracaoFiscal == null)
                {
                    resposta.Success = false;
                    resposta.Message = "Não existe configuração fiscal cadastrada";
                }

                resposta.Success = true;
                resposta.Message = "Configuração fiscal cadastrada com sucesso";
                resposta.Data = configuracaoFiscal;
                return resposta;
            }
        }
    }
}