using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.MenuRole.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.MenuRole.Commands
{
    public class CreateMenuRoleCommand : IRequest<BaseCommandResponse>
    {
        public MenuRoleCommandBase[] command { get; set; }
        public class CreateMenuRoleCommandHandle : IRequestHandler<CreateMenuRoleCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            //  private readonly IValidator<CreateMenuRoleCommand> _validator;
            public CreateMenuRoleCommandHandle(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<BaseCommandResponse> Handle(CreateMenuRoleCommand request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();

                if (request.command == null || request.command.Any(c => c.MenuId == 0 || string.IsNullOrEmpty(c.RoleId)))
                {
                    resposta.Message = "Erro ao validar o cadastro do menu role";
                    resposta.Success = false;
                    return resposta;
                }

                var menuRoles = request.command.Select(c => new Hotel.Domain.Entities.MenuRole
                {
                    MenuId = c.MenuId,
                    RoleId = c.RoleId
                }).ToArray();

                await _unitOfWork.MenuRole.InsertMenuAsync(menuRoles);
                resposta.Success = true;
                resposta.Data = menuRoles;
                resposta.Message = "Dados inserido com sucesso";
                return resposta;
            }
        }
    }
}