using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Hospedagem.Queries
{
    public class GetHospedagemByIdQuery: IRequest<BaseCommandResponse>
    {
        public int Id { get; set; }

        public class GetHospedagemByIdQueryHandler : IRequestHandler<GetHospedagemByIdQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetHospedagemByIdQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetHospedagemByIdQuery request, CancellationToken cancellationToken)
            {
                
                var response = new BaseCommandResponse();
               
                var hospedagem = await _unitOfWork.Hospedagem.GetByIdAsync(request.Id);
                if (hospedagem is null)
                {
                    response.Message = "Dado(s) não encontrado";
                    response.Success = false;
                    return response;
                }

                response.Data = hospedagem;
                response.Success = true;
                response.Message = "Dado(s) carregado com sucesso";
                return   await Task.FromResult(response);
            }
        }
    }
}