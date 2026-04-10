using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.ConfiguracaoFiscal.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.ConfiguracaoFiscal.Commands
{
    public class UpdateConfiguracaoFiscalCommand : ConfiguracaoFiscalBase
    {
        public int Id { get; set; }

        public class UpdateConfiguracaoFiscalCommandHandler : IRequestHandler<UpdateConfiguracaoFiscalCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;


            public UpdateConfiguracaoFiscalCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
              
            }

            public async Task<BaseCommandResponse> Handle(UpdateConfiguracaoFiscalCommand request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var paramExiste = await _unitOfWork.Param.Get(request.Id);
                if (paramExiste == null)
                {
                    resposta.Message = "Registro não encontrado";
                    resposta.Success = false;
                    return resposta;
                }
                var param = new Param(request.Taxa, request.CalcularTaxa, request.CalcularHora, request.Tolerancia, request.Regime, request.SistemaContabilistico, request.DataInicio, request.DataFim, request.Estabelecimento, request.Isencao, request.IVA, request.RegistroPorPagina, request.TipoRecibo,request.NomeEmpresa,request.Endereco,request.Cidade,request.NumContribuinte,request.Telefone, request.Email,request.LogoCaminho,request.ContaBancaria);
                param.Codigo(request.Id);

                await _unitOfWork.Param.Update(param);

                // Notificar atualização do rack após mudanças na configuração fiscal
               

                resposta.Data = param;
                resposta.Message = "Dados inserido com sucesso";
                resposta.Success = true;
                return resposta;
            }
        }
    }
}