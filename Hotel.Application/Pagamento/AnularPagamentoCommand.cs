using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Pagamento
{
    public class AnularPagamentoCommand: IRequest<BaseCommandResponse>
    {
        public int PagamentoId { get; set; }
        public class AnularPagamentoCommandHandler : IRequestHandler<AnularPagamentoCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly UsuarioLogado _logado;


            public AnularPagamentoCommandHandler(IUnitOfWork unitOfWork, UsuarioLogado logado)
            {
                _unitOfWork = unitOfWork;
                _logado = logado;
            }

            public async  Task<BaseCommandResponse> Handle(AnularPagamentoCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();


                try
                {
                    var pagamento = await _unitOfWork.pagamentos.GetByIdAsync(request.PagamentoId);
                     if (pagamento == null)
                        return RespostaErro("Pagamento não encontrado.");

                   
                    

                     var lancamentoCaixa = await _unitOfWork.lancamentoCaixa.GetByPagamentoIdAsync(pagamento.Id);
                     if (lancamentoCaixa == null)
                        return RespostaErro("Lançamento de caixa não encontrado.");

                 var caixa = await _unitOfWork.caixa.GetByIdAsync(lancamentoCaixa.CaixasId);
                
                if (caixa == null)
                 return RespostaErro("Caixa não encontrado.");

                    caixa.AdicionarSaida(pagamento.Valor);
                    await _unitOfWork.caixa.Update(caixa);

                var historico = new Historico(lancamentoCaixa.CaixasId, _logado.IdUtilizador, pagamento?.OrigemId ?? 0);
                    historico.AdicionarObservacao("Foi anulado - " + lancamentoCaixa.Observacao + " no valor de. " + lancamentoCaixa.ValorPago + " realizado no dia " + lancamentoCaixa.DataHoraLancamento);
                    await _unitOfWork.historico.Add(historico);

                     await _unitOfWork.lancamentoCaixa.Delete(lancamentoCaixa);
                     await _unitOfWork.pagamentos.Delete(pagamento);

                var checkin = await _unitOfWork.checkins.GetByIdAsync(pagamento.OrigemId); // .GetCheckinById(request.checkinId);
                     if (checkin == null)
                        return RespostaErro("Check-in não encontrado.");

                     checkin.ActualizarSituacaoDoPagamento();
                     await _unitOfWork.checkins.Update(checkin);

                    response.Success = true;
                    response.Message = "Pagamento anulado com sucesso.";

                } catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = $"Erro ao anular o pagamento: {ex.Message}";
                }

                return response;
            }
             private BaseCommandResponse RespostaErro(string mensagem)
            {
                return new BaseCommandResponse
                {
                    Success = false,
                    Message = mensagem
                };
            }
        }
    }
}