using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;

namespace Hotel.Application.TaxAccountingBasis.Queries
{
    public class GetTaxAccountingBasisQuery: IRequest<BaseCommandResponse>
    {
        public class TaxAccountingBasisQueryHandler: IRequestHandler<GetTaxAccountingBasisQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public TaxAccountingBasisQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetTaxAccountingBasisQuery request, CancellationToken cancellationToken) 
            {
                var resposta = new BaseCommandResponse();
                var data = await _unitOfWork.TaxAccountingBase.GetAllAsync();

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