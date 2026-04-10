using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.Extensions;
using Hotel.Application.Caixa.Commands;
using Hotel.Application.LancamentoCaixa.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Identity.Client;
using Hotel.Application.Services;

namespace Hotel.Application.LancamentoCaixa.Commands
{
    public class CreateLancamentoCaixaCommand : LancamentoCaixacommandBase
    {
        public class CreateLancamentoCaixaCommandHandler : IRequestHandler<CreateLancamentoCaixaCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly UsuarioLogado _usuario;
            private readonly IValidator<CreateLancamentoCaixaCommand> validator;
            private readonly IMediator _mediator;

            public CreateLancamentoCaixaCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateLancamentoCaixaCommand> validator, IMediator mediator, UsuarioLogado usuariologado)
            {
                _unitOfWork = unitOfWork;
                this.validator = validator;
                _mediator = mediator;
                _usuario = usuariologado;
            }

            public async Task<BaseCommandResponse> Handle(CreateLancamentoCaixaCommand request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var validationResult = await validator.ValidateAsync(request);
                try
                {
                    if (!validationResult.IsValid)
                    {
                        resposta.Success = false;
                        resposta.Message = "Erro ao validar o lançamento de caixa";
                        resposta.Errors = validationResult.Errors.Select(m => m.ErrorMessage).ToList();
                    }
                    else
                    {
                        var IdCaixa = await _unitOfWork.caixa.getCaixa();
                        await _mediator.Send(new AdicionarEntradaCommand { Id = IdCaixa, Valor = request.ValorPago, SaldoInicial = 0 });

                        var lancamentoCaixa = new Domain.Entities.LancamentoCaixa(request.Valor, request.DataPagamento, request.DataPagamento, request.TipoPagamentosId, request.PagamentosId, IdCaixa, Domain.Enums.TipoLancamento.E, "", request.PlanoDeContasId, _usuario.UserId);
                       // lancamentoCaixa.UtilizadoresId = _usuario.UserId;
                        // lancamentoCaixa.UtilizadoresId = User
                        //  lancamentoCaixa.CaixasId = 3;
                        lancamentoCaixa.DefinirValorPago(request.ValorPago);
                        lancamentoCaixa.CalcularTroco();

                        await _unitOfWork.lancamentoCaixa.Add(lancamentoCaixa);
                        //  await _unitOfWork.Save();

                        resposta.Data = lancamentoCaixa;
                        resposta.Success = true;
                        resposta.Message = "Lancamento feito com sucesso";
                    }

                }
                catch (Exception ex)
                {
                    resposta.Success = false;
                    resposta.Message = $"Erro ao lançar pagamento {ex.Message}";
                }
                return await Task.FromResult(resposta);
            }
        }

    }
}