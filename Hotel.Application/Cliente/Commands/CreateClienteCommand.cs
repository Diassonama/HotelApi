using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.Cliente.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Cliente.Commands
{
    public class CreateClienteCommand : ClienteCommandBase
    {
        public class CreateClienteCommandHandler : IRequestHandler<CreateClienteCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IValidator<CreateClienteCommand> _validator;
            public CreateClienteCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateClienteCommand> validator)
            {
                _unitOfWork = unitOfWork;
                _validator = validator;
            }

            public async Task<BaseCommandResponse> Handle(CreateClienteCommand request, CancellationToken cancellationToken)
            {
                BaseCommandResponse response = new BaseCommandResponse();

                var validateResult = await _validator.ValidateAsync(request);

                try
                {

                    if (!validateResult.IsValid)
                    {
                        response.Success = false;
                        response.Message = "Erros encontrado ao cadastrar cliente";
                        response.Errors = validateResult.Errors.Select(o => o.ErrorMessage).ToList();
                    }
                    else
                    {
                        var cliente = new Domain.Entities.Cliente(request.Nome, request.Email, request.Generos, request.DataAniversario, request.Telefone, request.EmpresasId, request.PaisId);

                        await _unitOfWork.clientes.Add(cliente);
                        // await _unitOfWork.Save();

                        // await _mediator.Publish(new ApartamentoCreateNotification(apartamento), cancellationToken);
                        response.Data = cliente;
                        response.Success = true;
                        response.Message = "Cliente cadastrado com sucesso";
                    };
                    //throw new NotImplementedException();
                }
                catch (Exception ex)
                {
                    // Tratamento de exceções
                    response.Success = false;
                    response.Message = $"Erro ao cadastrar cliente: {ex.Message}";
                }
                return await Task.FromResult(response);


            }
        }

    }
}