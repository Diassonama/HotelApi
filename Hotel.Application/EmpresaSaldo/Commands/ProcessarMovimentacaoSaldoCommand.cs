using System.ComponentModel.DataAnnotations;
using Hotel.Application.EmpresaSaldo.Base;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Enums;
using Hotel.Domain.Interface;
using MediatR;
using Serilog;

namespace Hotel.Application.EmpresaSaldo.Commands
{
    public class ProcessarMovimentacaoSaldoCommand : EmpresaSaldoCommandBase
    {

        [Required(ErrorMessage = "Tipo de lançamento é obrigatório")]
        public TipoLancamento TipoLancamento { get; set; }

        public class ProcessarMovimentacaoSaldoCommandHandler : IRequestHandler<ProcessarMovimentacaoSaldoCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
             private readonly UsuarioLogado _usuarioLogado;

            public ProcessarMovimentacaoSaldoCommandHandler(IUnitOfWork unitOfWork, UsuarioLogado usuarioLogado)
            {
                _unitOfWork = unitOfWork;
                _usuarioLogado = usuarioLogado;
            }

            public async Task<BaseCommandResponse> Handle(ProcessarMovimentacaoSaldoCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();

                try
                {
                    Log.Information("Processando movimentação de saldo - EmpresaId: {EmpresaId}, Valor: {Valor}, Tipo: {Tipo}",
                        request.EmpresaId, request.Valor, request.TipoLancamento);

                    // Validar se empresa existe
                    var empresa = await _unitOfWork.Empresa.Get(request.EmpresaId);
                    if (empresa == null)
                    {
                        response.Success = false;
                        response.Message = $"Empresa {request.EmpresaId} não encontrada";
                        response.Errors = new List<string> { "Empresa inválida" };
                        return response;
                    }

                    // Processar movimentação no repositório
                    await _unitOfWork.EmpresaSaldo.ProcessarMovimentacaoSaldoAsync(
                        request.EmpresaId,
                        request.Valor,
                        request.TipoLancamento,
                        request.Documento,
                        _usuarioLogado.IdUtilizador,
                        request.Observacao);

                    response.Success = true;
                    response.Message = "Movimentação processada com sucesso";
                    // response.Id = request.EmpresaId;

                    Log.Information("Movimentação de saldo processada com sucesso - EmpresaId: {EmpresaId}", request.EmpresaId);

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
                    Log.Error(ex, "Erro ao processar movimentação de saldo");
                    response.Success = false;
                    response.Message = "Erro ao processar movimentação";
                    response.Errors = new List<string> { ex.Message };
                    return response;
                }
            }
        }
    }
}