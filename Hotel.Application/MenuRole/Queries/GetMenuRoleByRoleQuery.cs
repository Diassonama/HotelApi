using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.MenuRole.Queries
{
    public class GetMenuRoleByIdQuery: IRequest<BaseCommandResponse>
    {
        public string Id { get; set; }

        public class GetMenuRoleByIdQueryHandler : IRequestHandler<GetMenuRoleByIdQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetMenuRoleByIdQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetMenuRoleByIdQuery request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var menuRole = await _unitOfWork.MenuRole.GetMenuItemByRoleAsync(request.Id);
                if(menuRole ==null){
                    resposta.Success = false;
                    resposta.Message = "Menu Role não encontrado";
                    //resposta.Data = null;
                    return resposta;
                }

                resposta.Data= menuRole;
                resposta.Success = true;
                resposta.Message = "Menu role encontrado";

                return resposta;
            }
        }

    }
}