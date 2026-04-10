using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.EmpresaSaldo.Base;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Enums;
using Hotel.Domain.Interface;
using MediatR;
using Serilog;

namespace Hotel.Application.EmpresaSaldo.Commands
{
    public class DebitarSaldoEmpresaCommand : EmpresaSaldoCommandBase
    {

        public class DebitarSaldoEmpresaCommandHandler : IRequestHandler<DebitarSaldoEmpresaCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
             private readonly UsuarioLogado _usuarioLogado;

            public DebitarSaldoEmpresaCommandHandler(IUnitOfWork unitOfWork, UsuarioLogado usuarioLogado)
            {
                _unitOfWork = unitOfWork;
                _usuarioLogado = usuarioLogado;
            }

            public async Task<BaseCommandResponse> Handle(DebitarSaldoEmpresaCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();

                try
                {
                    Log.Information("Debitando saldo da empresa - EmpresaId: {EmpresaId}, Valor: {Valor}",
                        request.EmpresaId, request.Valor);

                    // Validar se empresa existe
                    var empresa = await _unitOfWork.Empresa.Get(request.EmpresaId);
                    if (empresa == null)
                    {
                        response.Success = false;
                        response.Message = $"Empresa {request.EmpresaId} não encontrada";
                        response.Errors = new List<string> { "Empresa inválida" };
                        return response;
                    }

                    // Debitar saldo (TipoLancamento.Debito)
                    await _unitOfWork.EmpresaSaldo.ProcessarMovimentacaoSaldoAsync(
                        request.EmpresaId,
                        request.Valor,
                        TipoLancamento.S,
                        request.Documento,
                        _usuarioLogado.IdUtilizador,
                        request.Observacao);

                    response.Success = true;
                    response.Message = "Saldo debitado com sucesso";

                    Log.Information("Saldo debitado com sucesso - EmpresaId: {EmpresaId}", request.EmpresaId);

                    return response;
                }
                catch (InvalidOperationException ex)
                {
                    Log.Error(ex, "Saldo insuficiente para empresa {EmpresaId}", request.EmpresaId);
                    response.Success = false;
                    response.Message = ex.Message;
                    response.Errors = new List<string> { ex.Message };
                    return response;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Erro ao debitar saldo");
                    response.Success = false;
                    response.Message = "Erro ao debitar saldo";
                    response.Errors = new List<string> { ex.Message };
                    return response;
                }
            }
        }
    }
}