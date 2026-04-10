using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.Empresa.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Empresa.Commands
{
    public class UpdateEmpresaCommand: EmpresaCommandBase
    {   public int Id { get; set; }
        public class UpdateEmpresaCommandHandler : IRequestHandler<UpdateEmpresaCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IValidator<UpdateEmpresaCommand> _validator;

            public UpdateEmpresaCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(UpdateEmpresaCommand request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var empresa = await _unitOfWork.Empresa.Get(request.Id);
                if (empresa == null)    
                {
                    resposta.Success = false;
                    resposta.Message = "Empresa não encontrada";
                    return resposta;
                }

                try
                {
                 empresa.Atualiza(request.Id,request.RazaoSocial,request.Email,request.Endereco,request.Bairro,request.Cidade, request.Telefone,request.PercentualDesconto,request.NumContribuinte);
                 await _unitOfWork.Empresa.Update(empresa);
                 resposta.Data = empresa;
                 resposta.Message="Empresa cadastrado com sucesso";
                 resposta.Success = true;
                // return resposta;
                }
                catch (Exception ex)
                {
                    resposta.Success = false;
                    resposta.Message =  $" Erro ao inserir Empresa {ex.Message}";
                  //  return resposta;
                }

                return resposta;

            }
        }
    }
}