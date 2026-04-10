using Hotel.Application.Empresa.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Hotel.Domain.Enums;

using Serilog;
using Hotel.Application.EmpresaSaldo.Base;
using Hotel.Application.Services;

namespace Hotel.Application.EmpresaSaldo.Commands
{
    public class AdicionarCreditoEmpresaCommand : EmpresaSaldoCommandBase
    {
        public class AdicionarCreditoEmpresaCommandHandler : IRequestHandler<AdicionarCreditoEmpresaCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
             private readonly UsuarioLogado _usuarioLogado;

            public AdicionarCreditoEmpresaCommandHandler(IUnitOfWork unitOfWork, UsuarioLogado usuarioLogado)
            {
                _unitOfWork = unitOfWork;
                _usuarioLogado = usuarioLogado;
            }

            public async Task<BaseCommandResponse> Handle(AdicionarCreditoEmpresaCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();

                try
                {
                    Log.Information("Adicionando crédito à empresa - EmpresaId: {EmpresaId}, Valor: {Valor}",
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

                    // Adicionar crédito (TipoLancamento.Credito)
                    await _unitOfWork.EmpresaSaldo.ProcessarMovimentacaoSaldoAsync(
                        request.EmpresaId,
                        request.Valor,
                        TipoLancamento.E,
                        request.Documento,
                        _usuarioLogado.IdUtilizador,
                        request.Observacao);

                    response.Success = true;
                    response.Message = "Crédito adicionado com sucesso";
                    //   response. = request.EmpresaId;

                    Log.Information("Crédito adicionado com sucesso - EmpresaId: {EmpresaId}", request.EmpresaId);

                    return response;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Erro ao adicionar crédito");
                    response.Success = false;
                    response.Message = "Erro ao adicionar crédito";
                    response.Errors = new List<string> { ex.Message };
                    return response;
                }
            }
        }

    }
}