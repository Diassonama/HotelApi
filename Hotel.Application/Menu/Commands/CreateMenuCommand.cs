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
    public class CreateMenuCommand : MenuCommandBase
    {
        public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IValidator<CreateMenuCommand> validator;

            public CreateMenuCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateMenuCommand> validator)
            {
                _unitOfWork = unitOfWork;
                this.validator = validator;
            }

            public async Task<BaseCommandResponse> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();
                var validateResult = await validator.ValidateAsync(request);
                try
                {

                    if (!validateResult.IsValid)
                    {
                        response.Success = false;
                        response.Message = "Erros encontrado ao fazer o cadastro";
                        response.Errors = validateResult.Errors.Select(o => o.ErrorMessage).ToList();
                    }
                    else
                    {
                        var menu = new AppMenu(request.PreIcon, request.PostIcon, request.Nome, request.Path);

                        await _unitOfWork.Menu.InsertMenuAsync(menu);

                        response.Data = menu;
                        response.Success = true;
                        response.Message = "Cadastrado feito com sucesso";
                    }
                }
                catch (Exception ex)
                {
                    // Tratamento de exceções
                    response.Success = false;
                    response.Message = $"Erro ao cadastrar menu: {ex.Message}";
                }
                //throw new NotImplementedException();
                return await Task.FromResult(response);
            }
        }
    }
}