using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.DTOs.Request;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Menu.Queries
{
    public class HaveAcessoQuery: IRequest<BaseCommandResponse>
    {
    // public HaveAccessRequest acesso {get;}
                public string Path { get; set; }
           public string RoleName { get; set; }

        public class HaveAcessoQueryHandler : IRequestHandler<HaveAcessoQuery, BaseCommandResponse>
        {
            public readonly IUnitOfWork  _unitOfWork;


            public HaveAcessoQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async  Task<BaseCommandResponse> Handle(HaveAcessoQuery request, CancellationToken cancellationToken)
            {
               var resposta = new BaseCommandResponse();
               var menu = await _unitOfWork.Menu.HaveAcess(request.Path, request.RoleName);
            
               if (menu){
                   resposta.Success = true;
                   resposta.Message = "Acesso Permitido";
                   resposta.Data = menu;
                   return resposta;
               }
               else
               {
                   resposta.Success = false;
                   resposta.Message = "Acesso Negado";
                   return resposta;
               }
            
            }
        }
    }
}