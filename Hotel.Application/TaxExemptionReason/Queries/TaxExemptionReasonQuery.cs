using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.TaxExemptionReason.Queries
{
    public class GetTaxExemptionReasonQuery: IRequest<BaseCommandResponse>
    {
        public class TaxExemptionReasonQueryHandler: IRequestHandler<GetTaxExemptionReasonQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public TaxExemptionReasonQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetTaxExemptionReasonQuery request, CancellationToken cancellationToken) 
            {
                var resposta = new BaseCommandResponse();
                var data = await _unitOfWork.TaxExemptionReason.GetAllAsync();

                if(data == null){
                    resposta.Success = false;
                    resposta.Message = "Não foi possível encontrar os dados";
                    return resposta;
                }

                resposta.Success = true;
                resposta.Data = data;
                resposta.Message = "Dados carregados com sucesso";
                return resposta;
            }

        }
        
    }
}