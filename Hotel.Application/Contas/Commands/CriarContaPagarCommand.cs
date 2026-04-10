// Hotel.Application/Contas/Commands/CriarContaPagarCommand.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using Hotel.Application.Contas.Base;

namespace Hotel.Application.Contas.Commands
{
    public class CriarContaPagarCommand : ContaCommandBase
    {
        public int? EmpresaId { get; set; }
        public string FornecedorNome { get; set; }


        public class Handler : IRequestHandler<CriarContaPagarCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _uow;
            public Handler(IUnitOfWork uow) { _uow = uow; }

            public async Task<BaseCommandResponse> Handle(CriarContaPagarCommand req, CancellationToken ct)
            {
                var resp = new BaseCommandResponse();
                try
                {
                    var conta = new Hotel.Domain.Entities.ContaPagar(req.Valor, DateTime.Now, req.Vencimento, req.Documento, req.FornecedorNome, req.EmpresaId, req.Observacao);
                    await _uow.ContasPagar.Add(conta);
                    await _uow.Save();
                    resp.Success = true; resp.Message = "Conta a pagar criada";
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