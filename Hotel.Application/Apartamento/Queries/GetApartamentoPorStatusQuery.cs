using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Apartamento.Queries
{
    public class GetApartamentoPorStatusQuery : IRequest<BaseCommandResponse>
    {
        public class GetApartamentoPorStatusQueryHandler : IRequestHandler<GetApartamentoPorStatusQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetApartamentoPorStatusQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetApartamentoPorStatusQuery request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();

                var (quantidadesPorStatus, total) = await _unitOfWork.Apartamento.ObterQuantidadeQuartosPorStatusAsync();

                if (quantidadesPorStatus == null || !quantidadesPorStatus.Any())
                {
                    resposta.Success = false;
                    resposta.Message = "Não foi possível obter os quartos";
                }
                else
                {
                    resposta.Success = true;
                    resposta.Message = "Quartos obtidos com sucesso";

                    // Inclui os dados e o total na resposta
                    resposta.Data = new
                    {
                        QuantidadesPorStatus = quantidadesPorStatus,
                        Total = total
                    };
                }
                return resposta;
            }
        }
    }
}