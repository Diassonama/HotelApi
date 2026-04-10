using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.Hospedes.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Hospedes.Commands
{
    public class CreateHospedeCommand : HospedeCommandBase
    {
        public class CreateHospedeCommandHandler : IRequestHandler<CreateHospedeCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMediator _mediator;
            private readonly IValidator<CreateHospedeCommand> _validator;

            public CreateHospedeCommandHandler(IUnitOfWork unitOfWork, IMediator mediator, IValidator<CreateHospedeCommand> validator)
            {
                _unitOfWork = unitOfWork;
                _mediator = mediator;
                _validator = validator;
            }

            public async Task<BaseCommandResponse> Handle(CreateHospedeCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();

                var validateResult = await _validator.ValidateAsync(request);
                try
                {

                    if (!validateResult.IsValid)
                    {
                        response.Success = false;
                        response.Message = "Erros encontrado ao cadastrar hospede";
                        response.Errors = validateResult.Errors.Select(o => o.ErrorMessage).ToList();
                    }
                    else
                    {
                        var cliente = await _unitOfWork.clientes.GetByIdAsync(request.clientesId);
                        if (cliente == null)
                            throw new ArgumentException("Cliente não encontrado");

                        EstadoHospede _Estado = new EstadoHospede();
                        _Estado = cliente.Empresa.RazaoSocial == "Conta Propria" ? EstadoHospede.ContaPropria : EstadoHospede.Empresa;

                        var hospede = new Hospede(request.clientesId, request.checkinsId, (Hospede.EstadoHospede)_Estado);

                        await _unitOfWork.hospedes.Add(hospede);
                        //  await _unitOfWork.Save();

                        response.Data = hospede;
                        response.Success = true;
                        response.Message = "hospede cadastrado com sucesso";
                    };
                    //throw new NotImplementedException();
                }
                catch (Exception ex)
                {
                    // Tratamento de exceções
                    response.Success = false;
                    response.Message = $"Erro ao cadastrar hospede: {ex.Message}";
                }
                return await Task.FromResult(response);
            }
        }

    }
}