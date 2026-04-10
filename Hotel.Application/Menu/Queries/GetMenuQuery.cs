using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Menu.Queries
{
    public class GetMenuQuery: IRequest<BaseCommandResponse>
    {
        public class GetMenuQueryHandler : IRequestHandler<GetMenuQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetMenuQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetMenuQuery request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();
                var menu = await _unitOfWork.Menu.GetMenuAsync();

                if(menu is null)
                {
                    response.Message = "Dado(s) não encontrado";
                    response.Success = false;
                    return response;
                }

                response.Data = menu;
                response.Success = true;
                response.Message = "Dado(s) carregado com sucesso";
                return  await Task.FromResult(response);
            }
        }
    }
}