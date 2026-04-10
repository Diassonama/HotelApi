using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.Empresa.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Empresa.Commands
{
    public class CreateEmpresaCommand: EmpresaCommandBase
    {
        public class CreateEmpresaCommandHandler : IRequestHandler<CreateEmpresaCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork  _unitOfWork;
            private readonly IValidator<CreateEmpresaCommand> _validator;

            public CreateEmpresaCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateEmpresaCommand> validator)
            {
                _unitOfWork = unitOfWork;
                _validator = validator;
            }

            public async Task<BaseCommandResponse> Handle(CreateEmpresaCommand request, CancellationToken cancellationToken)
            {
                 var resposta = new BaseCommandResponse();
                try
                {
                   
                    var validateResult = await _validator.ValidateAsync(request);

                    if (!validateResult.IsValid){
                        
                        
                        resposta.Success = false;
                        resposta.Message = "Erros encontrado ao cadastrar Empresa";
                        resposta.Errors = validateResult.Errors.Select(o => o.ErrorMessage).ToList();
                        return resposta;
                    }else
                    {
                     var novaEmpresa = new Domain.Entities.Empresa(request.RazaoSocial,request.Telefone,request.Endereco,request.Email, request.PercentualDesconto,request.NumContribuinte);  

                      await _unitOfWork.Empresa.Add(novaEmpresa);

                      resposta.Success = true;
                      resposta.Message = "Empresa cadastrada com sucesso";
                      resposta.Data = novaEmpresa;
                      return resposta;

                    }
                    
                }
                catch (Exception ex)
                {
                    resposta.Success = false;
                    resposta.Message = $"Erro ao cadastrar Empresa {ex.Message}";
                }

                return resposta;
            }
        }
    }
}