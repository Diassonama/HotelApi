using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Menu.Queries
{
    public class GetMenuByRoleQuery : IRequest<BaseCommandResponse>
    {
        public string Id { get; set; }



        public class GetMenuAcessobyRoleAsyncHandler : IRequestHandler<GetMenuByRoleQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetMenuAcessobyRoleAsyncHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetMenuByRoleQuery request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();

                var menu = await _unitOfWork.Menu.GetMenuByRoleAsync(request.Id);

                if (menu == null)
                {
                    response.Success = false;
                    response.Message = "Menu não encontrado";
                    return response;
                }
                else
                {

                    response.Success = true;
                    response.Data = menu;
                    response.Message = "Menu carregado com sucesso ";
                    return response;
                }
            }
        }

    }
}