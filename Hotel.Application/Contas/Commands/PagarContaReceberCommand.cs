using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums;
using Hotel.Application.Services;

namespace Hotel.Application.Contas.Commands
{
    public class PagarContaReceberCommand : IRequest<BaseCommandResponse>
    {
        public int ContaReceberId { get; set; }
        public decimal Valor { get; set; }
        public int TipoPagamentosId { get; set; }
      //  public int CaixasId { get; set; }
       // public int PlanoDeContasId { get; set; }
        public string Observacao { get; set; }
       // public string UtilizadoresId { get; set; }

        public class Handler : IRequestHandler<PagarContaReceberCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _uow;
              private readonly UsuarioLogado _usuario;

            public Handler(IUnitOfWork uow, UsuarioLogado usuario)
            {
                _uow = uow;
                _usuario = usuario;
            }

            public async Task<BaseCommandResponse> Handle(PagarContaReceberCommand req, CancellationToken ct)
            {
                var resp = new BaseCommandResponse();
                try
                {
                    var conta = await _uow.ContasReceber.Get(req.ContaReceberId);
                    if (conta == null)
                    {
                        resp.Success = false; resp.Message = "Conta não encontrada"; return resp;
                    }

                    var planoDeConta = await _uow.PlanoDeContas.GetByNameAsync("Conta Receber");
                    if (planoDeConta == null)
                    {
                        resp.Success = false; resp.Message = "Plano de Conta 'Conta Receber' não encontrado"; return resp;
                    }

                    var IdCaixa = await _uow.caixa.getCaixa();


                    conta.RegistrarPagamento(req.Valor);
                    await _uow.ContasReceber.Update(conta);

                    //aqui chama o command pagamgento
                    var novoPagamento = new Domain.Entities.Pagamento(
                        valor: (float)req.Valor,
                        dataVencimento: DateTime.Now,
                        hospedesId: 170,
                      //  checkinsId: conta.CheckinsId ?? 0,
                        utilizadoresId: _usuario.UserId,
                        origem: "Conta Receber",
                        origemId: conta.Id
                        );  
                        novoPagamento.ConfirmarPagamento();
                    await _uow.pagamentos.Add(novoPagamento);

                    // Lançar no caixa (entrada)
                    var lanc = new Domain.Entities.LancamentoCaixa(
                        valor: (float)req.Valor,
                        dataHoraLancamento: DateTime.Now,
                        dataHoraVencimento: DateTime.Now,
                        tipoPagamentosId: req.TipoPagamentosId,
                        pagamentosId: novoPagamento.Id,
                        caixasId: IdCaixa ,//req.CaixasId,
                        tipoLancamento: TipoLancamento.E,
                        observacao: req.Observacao ?? $"Recebimento CR #{conta.Id}",
                        planoDeContasId: planoDeConta.Id,
                        utilizadoresId: _usuario.IdUtilizador
                    );
                    lanc.DefinirValorPago((float)req.Valor);
                    lanc.CalcularTroco();
                    await _uow.lancamentoCaixas.Add(lanc);

                    await _uow.Save();
                    resp.Success = true; resp.Message = "Pagamento registrado e lançado no caixa";
                    return resp;
                }
                catch (Exception ex)
                {
                    resp.Success = false; resp.Message = ex.Message;
                    return resp;
                }
            }
        }
    }
}