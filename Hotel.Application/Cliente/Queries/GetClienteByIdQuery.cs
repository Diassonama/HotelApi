using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Cliente.Queries
{
    public class GetClienteByIdQuery: IRequest<BaseCommandResponse>
    {
        public int Id { get; set; }

        public class GetClienteByIdQueryHandler : IRequestHandler<GetClienteByIdQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetClienteByIdQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();
                var existingData = await _unitOfWork.clientes.GetByIdAsync(request.Id);
                
                if (existingData is null)
                {
                    response.Message = "Dado(s) não encontrado";
                    response.Success = false;
                    return response;
                }

                response.Data = existingData;
                response.Success = true;
                response.Message = "Dado(s) carregado com sucesso";
                return   await Task.FromResult(response);
            }
        }
    }
}