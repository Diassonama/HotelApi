using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using Hotel.Application.Contas.Base;

namespace Hotel.Application.Contas.Commands
{
    public class CriarContaReceberEmpresaCheckoutCommand : ContaCommandBase
    {
        public int CheckinId { get; set; }
        public int EmpresaId { get; set; }


        public class Handler : IRequestHandler<CriarContaReceberEmpresaCheckoutCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _uow;
            public Handler(IUnitOfWork uow) { _uow = uow; }

            public async Task<BaseCommandResponse> Handle(CriarContaReceberEmpresaCheckoutCommand req, CancellationToken ct)
            {
                var resp = new BaseCommandResponse();
                try
                {
                    var empresa = await _uow.Empresa.Get(req.EmpresaId);
                    var checkin = await _uow.checkins.Get(req.CheckinId);
                    if (empresa == null || checkin == null)
                    {
                        resp.Success = false; resp.Message = "Empresa ou Checkin inválido"; return resp;
                    }

                    var conta = new Hotel.Domain.Entities.ContaReceber(req.EmpresaId, req.Valor, DateTime.Now, req.Vencimento, req.Documento, req.CheckinId, req.Observacao);
                    await _uow.ContasReceber.Add(conta);
                    await _uow.Save();

                    resp.Success = true; resp.Message = "Conta a receber criada";
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