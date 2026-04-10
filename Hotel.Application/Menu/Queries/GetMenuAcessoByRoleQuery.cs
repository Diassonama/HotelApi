using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Menu.Queries
{
    public class GetMenuAcessoByRoleQuery: IRequest<BaseCommandResponse>
    {
        public string Id { get; set; }

        public class GetMenuAcessoByRoleQueryHandler : IRequestHandler<GetMenuAcessoByRoleQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetMenuAcessoByRoleQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetMenuAcessoByRoleQuery request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var menuExiste = await _unitOfWork.Menu.GetMenuAcessobyRoleAsync(request.Id);

                if (menuExiste == null){
                    resposta.Success = false;
                    resposta.Message = "Menu não encontrado";
                    return resposta;
                }

                resposta.Success = true;    
                resposta.Data = menuExiste;
                resposta.Message = "Menu encontrado com sucesso";
                return resposta;

            }
        }
    }
}