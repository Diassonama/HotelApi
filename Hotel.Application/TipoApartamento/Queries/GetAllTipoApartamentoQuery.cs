using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.TipoApartamento.Queries
{
    public class GetAllTipoApartamentoQuery: IRequest<BaseCommandResponse>
    {
        public class GetAllTipoApartamentoQueryHandler : IRequestHandler<GetAllTipoApartamentoQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetAllTipoApartamentoQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetAllTipoApartamentoQuery request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();
                var tipoApartamento = await _unitOfWork.TipoApartamento.GetApartamentoAsync();

                if(tipoApartamento is null)
                {
                    response.Message = "Dado(s) não encontrado";
                    response.Success = false;
                    return response;
                }

                response.Data = tipoApartamento;
                response.Success = true;
                response.Message = "Dado(s) carregado com sucesso";
                return   await Task.FromResult(response);
            }
        }
    }
}