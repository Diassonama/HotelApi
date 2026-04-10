using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Enums;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Apartamento.Queries
{
    public class GetApartamentoFiltroSituacaoQuery: IRequest<BaseCommandResponse>
    {
        public Situacao Situacao { get; set; }
        public class GetApartamentoFiltroSituacaoQueryHandler : IRequestHandler<GetApartamentoFiltroSituacaoQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetApartamentoFiltroSituacaoQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetApartamentoFiltroSituacaoQuery request, CancellationToken cancellationToken)
            {
                 var response = new BaseCommandResponse();
                var existingApartamento = await _unitOfWork.Apartamento.GetBySituacaoAsync(request.Situacao);
                
                if (existingApartamento == null)
                {
                    response.Message = "Dado(s) não encontrado";
                    response.Success = false;
                    return response;
                }

                response.Data = existingApartamento;
                response.Success = true;
                response.Message = "Dado(s) carregado com sucesso";
                return   response; // await Task.FromResult(response);
            }
        }
    }
}