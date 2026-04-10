using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.ConfiguracaoFiscal.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.ConfiguracaoFiscal.Commands
{
    public class CreateConfiguracaoFiscalCommand: ConfiguracaoFiscalBase
    {
        public class CreateConfiguracaoFiscalCommandHandler : IRequestHandler<CreateConfiguracaoFiscalCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IValidator<CreateConfiguracaoFiscalCommand> _validator;

           

            public CreateConfiguracaoFiscalCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateConfiguracaoFiscalCommand> validator)
            {
                _unitOfWork = unitOfWork;
                _validator = validator;
            }

            public async Task<BaseCommandResponse> Handle(CreateConfiguracaoFiscalCommand request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var validationResult = await _validator.ValidateAsync(request);

                if (!validationResult.IsValid){
                    resposta.Success = false;
                    resposta.Message="Erro ao inserir Configuração";
                    resposta.Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
                    return resposta;
                }

                var param = new Param(request.Taxa,request.CalcularTaxa,request.CalcularHora,request.Tolerancia,request.Regime,request.SistemaContabilistico,request.DataInicio,request.DataFim,request.Estabelecimento,request.Isencao,request.IVA,request.RegistroPorPagina,request.TipoRecibo,request.NomeEmpresa,request.Endereco,request.Cidade,request.NumContribuinte,request.Telefone, request.Email,request.LogoCaminho,request.ContaBancaria);            
                await _unitOfWork.Param.Add(param);
                
                resposta.Success= true;
                resposta.Message="Configuração inserida com sucesso";
                resposta.Data = param;
                return resposta;
            }
        }
    }
}