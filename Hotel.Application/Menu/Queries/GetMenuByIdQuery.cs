using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Menu.Queries
{
    public class GetMenuByIdQuery: IRequest<BaseCommandResponse>
    {
        public int Id { get; set; }

        public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetMenuByIdQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();
                var menu = await _unitOfWork.Menu.GetMenuByIdAsync(request.Id);

                if(menu == null ){
                   
                    response.Success = false;
                    response.Message ="Dados não encontrado";
                    return response;
                }
                else
                {
                    response.Data = menu;
                    response.Success = true;
                    response.Message = "Dados carregado com sucesso";
                }

                return await Task.FromResult(response);
            }
        }
    }
}