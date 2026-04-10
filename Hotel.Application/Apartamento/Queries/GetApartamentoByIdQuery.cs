using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Apartamentos.Queries
{
    public class GetApartamentoByIdQuery: IRequest<BaseCommandResponse>
    {
        public int Id { get; set; }
        

        

        public class GetApartamentoByIdQueryHandler : IRequestHandler<GetApartamentoByIdQuery, BaseCommandResponse>
        {
             private IUnitOfWork _unitOfWork;

            public GetApartamentoByIdQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetApartamentoByIdQuery request, CancellationToken cancellationToken)
            {

                var response = new BaseCommandResponse();
                var existingApartamento = await _unitOfWork.Apartamento.GetByIdAsync(request.Id);
                
                if (existingApartamento is null)
                {
                    response.Message = "Dado(s) não encontrado";
                    response.Success = false;
                    return response;
                }

                response.Data = existingApartamento;
                response.Success = true;
                response.Message = "Dado(s) carregado com sucesso";
                return   await Task.FromResult(response); //await _unitOfWork.Apartamento.GetByIdAsync(request.Id);

               // throw new NotImplementedException();
            }
        }
    }
}