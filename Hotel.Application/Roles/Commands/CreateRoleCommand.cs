using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Application.Roles.Commands
{
    public class CreateRoleCommand : IRequest<BaseCommandResponse>
    {
        public string Nome { get; set; }
        public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IRoleRepository _repository;
            //  private readonly RoleManager<IdentityRole> _roleManager;
            private readonly IValidator<CreateRoleCommand> _validator;

            public CreateRoleCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateRoleCommand> validator, IRoleRepository repository)
            {
                _unitOfWork = unitOfWork;
                _validator = validator;
                _repository = repository;
                //       _roleManager = roleManager;
            }



            public async Task<BaseCommandResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var validationResult = await _validator.ValidateAsync(request);
                try
                {
                    if (!validationResult.IsValid)
                    {
                        resposta.Success = false;
                        resposta.Message = "Dados inválidos";
                        return resposta;
                    }
                    //  var count = await _unitOfWork.Perfil.Count();

                    var role = new IdentityRole { Name = request.Nome };    //("1", request.Nome);

                    await _repository.AddAsync(role);

                    resposta.Data = role;
                    resposta.Message = "Dados inserido com sucesso";
                }
                catch (Exception ex)
                {
                    // Tratamento de exceções
                    resposta.Success = false;
                    resposta.Message = $"Erro ao criar perfil: {ex.Message}";
                }
                return resposta;

            }
        }
    }
}