using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Apartamento.Queries
{
    public class GetApartamentosAtrasadoQuery: IRequest<BaseCommandResponse>
    {
       

        public class GetApartamentosAtrasadoQueryHandler : IRequestHandler<GetApartamentosAtrasadoQuery, BaseCommandResponse>
        {
             private readonly IUnitOfWork _unitOfWork;

            public GetApartamentosAtrasadoQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetApartamentosAtrasadoQuery request, CancellationToken cancellationToken)
            {
                 var resposta = new BaseCommandResponse();

                var apartamentos = await _unitOfWork.Apartamento.GetQuartosAtrazadosAsync();

                if (apartamentos == null)
                {
                    resposta.Success = false;
                    resposta.Message = "Não foi possível obter os quartos";
                }
                else
                {
                    resposta.Success = true;
                    resposta.Message = "Quartos obtidos com sucesso";
                    resposta.Data = apartamentos;
                }
                return resposta;
            }
        }

    }
}