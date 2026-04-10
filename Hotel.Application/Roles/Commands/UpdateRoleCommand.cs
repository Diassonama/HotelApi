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
    public class UpdateRoleCommand : IRequest<BaseCommandResponse>
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        
        public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
        {
            public UpdateRoleCommandValidator()
            {
                RuleFor(x => x.Id)
                    .NotEmpty()
                    .WithMessage("ID do perfil é obrigatório.");

                RuleFor(x => x.Nome)
                    .NotEmpty()
                    .WithMessage("Nome do perfil é obrigatório.")
                    .MaximumLength(256)
                    .WithMessage("Nome do perfil não pode exceder 256 caracteres.");
            }
        }

        public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IRoleRepository _repository;
            private readonly IValidator<UpdateRoleCommand> _validator;

            public UpdateRoleCommandHandler(IUnitOfWork unitOfWork, IValidator<UpdateRoleCommand> validator, IRoleRepository repository)
            {
                _unitOfWork = unitOfWork;
                _validator = validator;
                _repository = repository;
            }

            public async Task<BaseCommandResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                
                try
                {
                    // Validação
                    var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                    if (!validationResult.IsValid)
                    {
                        resposta.Success = false;
                        resposta.Message = "❌ Dados inválidos";
                        resposta.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                        return resposta;
                    }

                    // Verificar se o perfil existe
                    var roleExistente = await _repository.GetByIdAsync(request.Id);
                    if (roleExistente == null)
                    {
                        resposta.Success = false;
                        resposta.Message = "❌ Perfil não encontrado";
                        resposta.Errors = new List<string> { $"Perfil com ID {request.Id} não foi encontrado no sistema" };
                        return resposta;
                    }

                    // Verificar se o nome já existe em outro perfil
                    var roleComMesmoNome = await _repository.GetByNameAsync(request.Nome);
                    if (roleComMesmoNome != null && roleComMesmoNome.Id != request.Id)
                    {
                        resposta.Success = false;
                        resposta.Message = "❌ Nome do perfil já existe";
                        resposta.Errors = new List<string> { $"Já existe outro perfil com o nome '{request.Nome}'" };
                        return resposta;
                    }

                    // Atualizar os dados
                    roleExistente.Name = request.Nome;
                    roleExistente.NormalizedName = request.Nome.ToUpperInvariant();

                    // Salvar as alterações
                    await _repository.UpdateAsync(roleExistente);

                    resposta.Success = true;
                    resposta.Message = "✅ Perfil atualizado com sucesso";
                    resposta.Data = new 
                    {
                        Id = roleExistente.Id,
                        Name = roleExistente.Name,
                        NormalizedName = roleExistente.NormalizedName
                    };
                }
                catch (Exception ex)
                {
                    resposta.Success = false;
                    resposta.Message = "❌ Erro interno do servidor ao atualizar perfil";
                    resposta.Errors = new List<string> { ex.Message };
                }

                return resposta;
            }
        }
    }
}
