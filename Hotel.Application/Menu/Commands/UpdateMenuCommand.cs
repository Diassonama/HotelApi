using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.Menu.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Menu.Commands
{
    public class UpdateMenuCommand: MenuCommandBase
    {
        public int Id { get; set; }
        public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
          
            public UpdateMenuCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async  Task<BaseCommandResponse> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var menuExiste = await _unitOfWork.Menu.GetMenuByIdAsync(request.Id);

                if(menuExiste == null){
                    resposta.Success = false;
                    resposta.Message = "Registro não encontrado";
                    return resposta;
                }
                
                var menu = new AppMenu(request.PreIcon,request.PostIcon,request.Nome,request.Path);
                menu.Update(request.Id);
                await _unitOfWork.Menu.UpdateMenuAsync(menu);

                resposta.Success= true;
                resposta.Message = "Menu atualizado com sucesso";
                resposta.Data = menu;
                return resposta;
            }
        }

    }
}