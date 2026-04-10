using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Caixa.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Caixa.Commands
{
    public class AdicionarSaidaCommand: CaixaCommandBase
    {
        public class AdicionarEntradaCommandHandler : IRequestHandler<AdicionarSaidaCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public AdicionarEntradaCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(AdicionarSaidaCommand request, CancellationToken cancellationToken)
            {
                 var response = new BaseCommandResponse();
                 var caixa = await _unitOfWork.caixa.GetByIdAsync(request.Id);
                
                if (caixa == null)
                {
                    response.Message = "Registro não encontrado";
                    response.Success = false;
                    return response;
                }

               // var caixa = new Domain.Entities.Caixa(request.SaldoInicial,1 );
               caixa.AdicionarSaida(request.Valor);

                await _unitOfWork.caixa.Update(caixa);
             //   await _unitOfWork.Save();

                response.Data = caixa;
                response.Success = true;
                response.Message = "caixa atualizado com sucesso";
              //  return response;
                return  await Task.FromResult(response);

            }
        }
    }
}