using FluentValidation;
using Hotel.Application.Interfaces;
using Hotel.Application.Pagamento.Base;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static Hotel.Domain.Entities.Checkins;

namespace Hotel.Application.Pagamento
{
    public class CreatePagamentoCommand : PagamentoCommandBase
    {
        public class CreatePagamentoCommandHandler : IRequestHandler<CreatePagamentoCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IValidator<CreatePagamentoCommand> validator;
            private readonly IMediator _mediator;
            private readonly UsuarioLogado _logado;
            private readonly ICaixa _caixa;
            private readonly ILogger<CreatePagamentoCommandHandler> _logger;

            public CreatePagamentoCommandHandler(
                IUnitOfWork unitOfWork, 
                IValidator<CreatePagamentoCommand> validator, 
                IMediator mediator, 
                UsuarioLogado logado, 
                ICaixa caixa,
                ILogger<CreatePagamentoCommandHandler> logger)
            {
                _unitOfWork = unitOfWork;
                this.validator = validator;
                _mediator = mediator;
                _logado = logado;
                _caixa = caixa;
                _logger = logger;
            }

            public async Task<BaseCommandResponse> Handle(CreatePagamentoCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();
                var correlationId = Guid.NewGuid().ToString("N")[..8]; // ID para rastrear esta operação

                try
                {
                    _logger.LogInformation("🔄 [PAGAMENTO-{CorrelationId}] INICIANDO processamento de pagamento", correlationId);
                    _logger.LogInformation("📋 [PAGAMENTO-{CorrelationId}] REQUEST: {Request}", correlationId, JsonSerializer.Serialize(new {
                        CheckinId = request.pagamentoRequest?.CheckinsId,
                        ValorPago = request.pagamentoRequest?.ValorPago,
                        DataPagamento = request.pagamentoRequest?.DataPagamento,
                        TipoPagamento = request.pagamentoRequest?.TipoPagamentosId,
                        HospedeId = request.pagamentoRequest?.HospedesId
                    }));

                    // ✅ 1. VALIDAÇÃO INICIAL
                    _logger.LogInformation("🔍 [PAGAMENTO-{CorrelationId}] Iniciando validação de entrada", correlationId);
                    var validationResult = await validator.ValidateAsync(request, cancellationToken);
                    if (!validationResult.IsValid)
                    {
                        _logger.LogWarning("❌ [PAGAMENTO-{CorrelationId}] Falha na validação: {Errors}", 
                            correlationId, 
                            string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

                        return new BaseCommandResponse
                        {
                            Success = false,
                            Message = "Erros encontrados ao fazer o pagamento.",
                            Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                        };
                    }
                    _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Validação de entrada passou", correlationId);

                    // ✅ 2. VALIDAÇÕES DE NEGÓCIO
                    _logger.LogInformation("🔍 [PAGAMENTO-{CorrelationId}] Iniciando validação de pré-requisitos", correlationId);
                    await ValidarPreRequisitos(request, correlationId);
                    _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Pré-requisitos validados", correlationId);

                    // ✅ 3. OBTER ENTIDADES NECESSÁRIAS
                    _logger.LogInformation("📂 [PAGAMENTO-{CorrelationId}] Obtendo entidades necessárias", correlationId);
                    
                    var checkin = await ObterCheckinValido(request.pagamentoRequest.CheckinsId, correlationId);
                    _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Check-in obtido: ID={CheckinId}, ValorTotal={ValorTotal}, SituacaoPagamento={Situacao}", 
                        correlationId, checkin.Id, checkin.ValorTotalFinal, checkin.situacaoDoPagamento);

                    var caixa = await ObterCaixaAtivo(correlationId);
                    _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Caixa obtido: ID={CaixaId}", correlationId, caixa.Id);

                    var planoDeContas = await ObterPlanoDeContas(correlationId);
                    _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Plano de contas obtido: ID={PlanoId}, Nome={Nome}", 
                        correlationId, planoDeContas.Id, planoDeContas.Descricao);

                    var hospedagem = await ObterHospedagemValida(checkin.Id, correlationId);
                    _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Hospedagem obtida: ID={HospedagemId}, Quarto={Quarto}", 
                        correlationId, hospedagem.Id, hospedagem.Apartamentos?.Codigo);

                    // ✅ 4. INICIAR TRANSAÇÃO
                    _logger.LogInformation("🔄 [PAGAMENTO-{CorrelationId}] Iniciando transação", correlationId);
                    await _unitOfWork.BeginTransactionAsync();
                    
                    try
                    {
                        // ✅ 5. CRIAR PAGAMENTO
                        _logger.LogInformation("💰 [PAGAMENTO-{CorrelationId}] Criando pagamento", correlationId);
                        var novoPagamento = await CriarPagamento(request, checkin, correlationId);
                        _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Pagamento criado: ID={PagamentoId}, Valor={Valor}", 
                            correlationId, novoPagamento.Id, novoPagamento.Valor);

                        // ✅ 6. SALVAR PAGAMENTO
                        _logger.LogInformation("💾 [PAGAMENTO-{CorrelationId}] Salvando pagamento no banco", correlationId);
                        await _unitOfWork.pagamentos.Add(novoPagamento);
                        await _unitOfWork.Save();
                        _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Pagamento salvo com sucesso", correlationId);

                        // ✅ 7. ATUALIZAR CHECKIN
                        _logger.LogInformation("🔄 [PAGAMENTO-{CorrelationId}] Atualizando check-in com pagamento", correlationId);
                        await AtualizarCheckinComPagamento(checkin, novoPagamento, correlationId);
                        _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Check-in atualizado", correlationId);

                        // ✅ 8. ATUALIZAR FATURA
                        _logger.LogInformation("📄 [PAGAMENTO-{CorrelationId}] Verificando e atualizando fatura", correlationId);
                        await AtualizarFacturaSeExistir(checkin.Id, correlationId);
                        _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Fatura processada", correlationId);

                        // ✅ 9. PROCESSAR MOVIMENTO DE CAIXA
                        _logger.LogInformation("🏦 [PAGAMENTO-{CorrelationId}] Processando movimento de caixa", correlationId);
                        await ProcessarMovimentoCaixa(request, caixa, novoPagamento.Id, hospedagem, checkin, correlationId);
                        _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Movimento de caixa processado", correlationId);

                        // ✅ 10. COMMIT DA TRANSAÇÃO
                        _logger.LogInformation("💾 [PAGAMENTO-{CorrelationId}] Fazendo commit da transação", correlationId);
                        await _unitOfWork.CommitAsync();
                      //  await transaction.CommitAsync();
                        _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Transação commitada com sucesso", correlationId);

                        // ✅ 11. RESPOSTA DE SUCESSO
                        response.Data = new
                        {
                            PagamentoId = novoPagamento.Id,
                            ValorPago = novoPagamento.Valor,
                            SituacaoPagamento = checkin.situacaoDoPagamento.ToString(),
                            Observacao = $"Pagamento processado para quarto {hospedagem.Apartamentos.Codigo}",
                            
                            CorrelationId = correlationId
                        };
                        response.Success = true;
                        response.Message = "Pagamento processado com sucesso.";

                        _logger.LogInformation("🎉 [PAGAMENTO-{CorrelationId}] PAGAMENTO PROCESSADO COM SUCESSO! PagamentoId={PagamentoId}", 
                            correlationId, novoPagamento.Id);
                    }
                    catch (Exception transactionEx)
                    {
                        _logger.LogError(transactionEx, "💥 [PAGAMENTO-{CorrelationId}] ERRO NA TRANSAÇÃO: {Message}", 
                            correlationId, transactionEx.Message);
                        
                        // ✅ ROLLBACK EM CASO DE ERRO NA TRANSAÇÃO
                        await _unitOfWork.RollBackAsync();
                        _logger.LogWarning("🔄 [PAGAMENTO-{CorrelationId}] Rollback realizado", correlationId);
                        
                        throw new Exception($"Erro durante processamento do pagamento: {transactionEx.Message}", transactionEx);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 [PAGAMENTO-{CorrelationId}] ERRO GERAL: {Message} | StackTrace: {StackTrace}", 
                        correlationId, ex.Message, ex.StackTrace);

                    // ✅ TRATAMENTO GLOBAL DE ERROS
                    response.Success = false;
                    response.Message = $"Erro ao realizar o pagamento: {ex.Message}";
                    response.Data = new { CorrelationId = correlationId };
                }

                return response;
            }

            // ✅ MÉTODOS AUXILIARES COM LOGGING

            private async Task ValidarPreRequisitos(CreatePagamentoCommand request, string correlationId)
            {
                _logger.LogInformation("🔍 [PAGAMENTO-{CorrelationId}] Validando usuário logado", correlationId);
                if (string.IsNullOrEmpty(_logado.IdUtilizador))
                {
                    _logger.LogError("❌ [PAGAMENTO-{CorrelationId}] Usuário não autenticado", correlationId);
                    throw new UnauthorizedAccessException("Usuário não está autenticado.");
                }
                _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Usuário válido: {UserId}", correlationId, _logado.IdUtilizador);

                _logger.LogInformation("🔍 [PAGAMENTO-{CorrelationId}] Validando caixa", correlationId);
                var caixaId = _caixa.getCaixa;
                _logger.LogInformation("📋 [PAGAMENTO-{CorrelationId}] CaixaId obtido: {CaixaId}", correlationId, caixaId);
                if (caixaId <= 0)
                {
                    _logger.LogError("❌ [PAGAMENTO-{CorrelationId}] Caixa fechado: {CaixaId}", correlationId, caixaId);
                    throw new InvalidOperationException("Caixa encontra-se fechado. Não é possível processar pagamentos.");
                }

                _logger.LogInformation("🔍 [PAGAMENTO-{CorrelationId}] Validando valor do pagamento: {Valor}", correlationId, request.pagamentoRequest.ValorPago);
                if (request.pagamentoRequest.ValorPago <= 0)
                {
                    _logger.LogError("❌ [PAGAMENTO-{CorrelationId}] Valor inválido: {Valor}", correlationId, request.pagamentoRequest.ValorPago);
                    throw new ArgumentException("Valor do pagamento deve ser maior que zero.");
                }

               /*  _logger.LogInformation("🔍 [PAGAMENTO-{CorrelationId}] Validando data do pagamento: {Data}", correlationId, request.pagamentoRequest.DataPagamento);
                if (request.pagamentoRequest.DataPagamento.Date > DateTime.Now)
                {
                    _logger.LogError("❌ [PAGAMENTO-{CorrelationId}] Data no futuro: {Data}", correlationId, request.pagamentoRequest.DataPagamento);
                    throw new ArgumentException("Data do pagamento não pode ser no futuro.");
                } */
            }

            private async Task<Domain.Entities.Checkins> ObterCheckinValido(int checkinId, string correlationId)
            {
                _logger.LogInformation("🔍 [PAGAMENTO-{CorrelationId}] Buscando check-in: {CheckinId}", correlationId, checkinId);
                
                var checkin = await _unitOfWork.checkins.GetByIdAsync(checkinId);
                
                if (checkin == null)
                {
                    _logger.LogError("❌ [PAGAMENTO-{CorrelationId}] Check-in não encontrado: {CheckinId}", correlationId, checkinId);
                    throw new NotFoundException($"Check-in com ID {checkinId} não encontrado.");
                }

                _logger.LogInformation("📋 [PAGAMENTO-{CorrelationId}] Check-in encontrado: ID={Id}, SituacaoPagamento={Situacao}, ValorTotal={Valor}", 
                    correlationId, checkin.Id, checkin.situacaoDoPagamento, checkin.ValorTotalFinal);

                if (checkin.situacaoDoPagamento == SituacaoDoPagamento.Pago)
                {
                    _logger.LogError("❌ [PAGAMENTO-{CorrelationId}] Pagamento já realizado para check-in: {CheckinId}", correlationId, checkinId);
                    throw new InvalidOperationException("O pagamento já foi realizado na sua totalidade.");
                }

                 if (checkin.situacaoDoPagamento == SituacaoDoPagamento.Pago)
                         throw new InvalidOperationException("O pagamento já foi realizado na sua totalidade.");


                var valorPago = checkin.Pagamentos?.Sum(p => p.Valor) ?? 0;
                var valorPendente = checkin.ValorTotalFinal - valorPago;
                _logger.LogInformation("💰 [PAGAMENTO-{CorrelationId}] Valores - Total: {Total}, Pago: {Pago}, Pendente: {Pendente}", 
                    correlationId, checkin.ValorTotalFinal, valorPago, valorPendente);
                    

                return checkin;
            }

            private async Task<Domain.Entities.Hospedagem> ObterHospedagemValida(int checkinId, string correlationId)
            {
                _logger.LogInformation("🔍 [PAGAMENTO-{CorrelationId}] Buscando hospedagem para check-in: {CheckinId}", correlationId, checkinId);
                
                var hospedagem = await _unitOfWork.Hospedagem.GetByCheckinIdAsync(checkinId);
                
                if (hospedagem == null)
                {
                    _logger.LogError("❌ [PAGAMENTO-{CorrelationId}] Hospedagem não encontrada para check-in: {CheckinId}", correlationId, checkinId);
                    throw new NotFoundException($"Hospedagem não encontrada para o CheckinId {checkinId}.");
                }

                if (hospedagem.Apartamentos == null)
                {
                    _logger.LogError("❌ [PAGAMENTO-{CorrelationId}] Apartamento não encontrado para hospedagem: {HospedagemId}", correlationId, hospedagem.Id);
                    throw new InvalidOperationException("Apartamento da hospedagem não encontrado.");
                }

                return hospedagem;
            }

            private async Task<Domain.Entities.Pagamento> CriarPagamento(CreatePagamentoCommand request, Domain.Entities.Checkins checkin, string correlationId)
            {
                _logger.LogInformation("💰 [PAGAMENTO-{CorrelationId}] Iniciando criação do pagamento", correlationId);
                
                // Validar se valor não excede o pendente
                var valorPago = checkin.Pagamentos?.Sum(p => p.Valor) ?? 0;
                var valorPendente = checkin.ValorTotalFinal - valorPago;
                
                _logger.LogInformation("📊 [PAGAMENTO-{CorrelationId}] Análise de valores - Solicitado: {Solicitado}, Pendente: {Pendente}", 
                    correlationId, request.pagamentoRequest.ValorPago, valorPendente);

                 if (request.pagamentoRequest.ValorPago > valorPendente)
                {
                    _logger.LogError("❌ [PAGAMENTO-{CorrelationId}] Valor excede pendente - Solicitado: {Solicitado}, Pendente: {Pendente}", 
                        correlationId, request.pagamentoRequest.ValorPago, valorPendente);
                    throw new InvalidOperationException($"Valor do pagamento (Kz {request.pagamentoRequest.ValorPago:F2}) excede o valor pendente (Kz {valorPendente:F2}).");
                } 
                try
                {
                    _logger.LogInformation("🏗️ [PAGAMENTO-{CorrelationId}] Construindo objeto pagamento", correlationId);
                    
                    var novoPagamento = new Domain.Entities.Pagamento(
                        request.pagamentoRequest.ValorPago,
                        request.pagamentoRequest.DataPagamento,
                        request.pagamentoRequest.HospedesId,
                     //   request.pagamentoRequest.CheckinsId,
                        _logado.IdUtilizador,
                        request.pagamentoRequest.Origem, 
                        request.pagamentoRequest.OrigemId
                        );

                    _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Objeto pagamento criado", correlationId);
                    
                    _logger.LogInformation("🔄 [PAGAMENTO-{CorrelationId}] Confirmando pagamento", correlationId);
                    novoPagamento.ConfirmarPagamento();
                    _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Pagamento confirmado", correlationId);

                    return novoPagamento;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 [PAGAMENTO-{CorrelationId}] Erro ao criar objeto pagamento: {Message}", correlationId, ex.Message);
                    throw;
                }
            }

            private async Task AtualizarCheckinComPagamento(Domain.Entities.Checkins checkin, Domain.Entities.Pagamento pagamento, string correlationId)
            {
                try
                {
                     _logger.LogInformation("🔄 [PAGAMENTO-{CorrelationId}] Registrando pagamento no check-in", correlationId);
        
        // ✅ CORREÇÃO: Verificar se pagamento já foi registrado antes de tentar adicionar
        if (pagamento.Id > 0 && checkin.PagamentoJaRegistrado(pagamento.Id))
        {
            _logger.LogWarning("⚠️ [PAGAMENTO-{CorrelationId}] Pagamento {PagamentoId} já registrado no check-in {CheckinId}", 
                correlationId, pagamento.Id, checkin.Id);
            return; // Não é erro, apenas já foi registrado
        }
                    _logger.LogInformation("🔄 [PAGAMENTO-{CorrelationId}] Registrando pagamento no check-in", correlationId);
                    checkin.RegistrarPagamento(pagamento);
                    
                    _logger.LogInformation("💾 [PAGAMENTO-{CorrelationId}] Atualizando check-in no banco", correlationId);
                    await _unitOfWork.checkins.Update(checkin);
                    _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Check-in atualizado", correlationId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 [PAGAMENTO-{CorrelationId}] Erro ao atualizar check-in: {Message}", correlationId, ex.Message);
                    throw new InvalidOperationException($"Erro ao registrar pagamento no check-in: {ex.Message}", ex);
                }
            }

            private async Task AtualizarFacturaSeExistir(int checkinId, string correlationId)
            {
                try
                {
                    _logger.LogInformation("🔍 [PAGAMENTO-{CorrelationId}] Buscando fatura para check-in: {CheckinId}", correlationId, checkinId);
                    var facturaEmpresa = await _unitOfWork.Factura.GetByCheckinsIdAsync(checkinId);
                    
                    if (facturaEmpresa != null)
                    {
                        _logger.LogInformation("📄 [PAGAMENTO-{CorrelationId}] Fatura encontrada: {FacturaId}", correlationId, facturaEmpresa.Id);
                        facturaEmpresa.Pagar();
                        await _unitOfWork.Factura.Update(facturaEmpresa);
                        _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Fatura atualizada", correlationId);
                    }
                    else
                    {
                        _logger.LogInformation("ℹ️ [PAGAMENTO-{CorrelationId}] Nenhuma fatura encontrada", correlationId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 [PAGAMENTO-{CorrelationId}] Erro ao atualizar fatura: {Message}", correlationId, ex.Message);
                    throw new InvalidOperationException($"Erro ao atualizar fatura: {ex.Message}", ex);
                }
            }

            private async Task ProcessarMovimentoCaixa(
                CreatePagamentoCommand request, 
                Domain.Entities.Caixa caixa, 
                int pagamentoId, 
                Domain.Entities.Hospedagem hospedagem,
                Domain.Entities.Checkins checkin,
                string correlationId)
            {
                try
                {
                    _logger.LogInformation("🏦 [PAGAMENTO-{CorrelationId}] Atualizando saldo do caixa", correlationId);
                    caixa.AdicionarEntrada((float)request.pagamentoRequest.ValorPago);
                    await _unitOfWork.caixa.Update(caixa);
                    _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Saldo do caixa atualizado", correlationId);

                    var observacao = checkin.situacaoDoPagamento == SituacaoDoPagamento.Pago 
                        ? $"PAG. TOTAL ({hospedagem.Apartamentos.Codigo})" 
                        : $"PAG. {SituacaoDoPagamento.Parcial} ({hospedagem.Apartamentos.Codigo})";

                    _logger.LogInformation("📝 [PAGAMENTO-{CorrelationId}] Criando lançamento no caixa: {Observacao}", correlationId, observacao);
                    var lancamentoCaixa = await CriarLancamentoCaixa(request, pagamentoId, caixa.Id, observacao, correlationId);
                    await _unitOfWork.lancamentoCaixa.Add(lancamentoCaixa);
                    _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Lançamento criado no caixa", correlationId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 [PAGAMENTO-{CorrelationId}] Erro ao processar movimento do caixa: {Message}", correlationId, ex.Message);
                    throw new InvalidOperationException($"Erro ao processar movimento do caixa: {ex.Message}", ex);
                }
            }

            private async Task<Domain.Entities.Caixa> ObterCaixaAtivo(string correlationId)
            {
                var caixaId = _caixa.getCaixa;
                _logger.LogInformation("🔍 [PAGAMENTO-{CorrelationId}] Buscando caixa: {CaixaId}", correlationId, caixaId);
                
                var caixa = await _unitOfWork.caixa.GetByIdAsync(caixaId);

                if (caixa == null)
                {
                    _logger.LogError("❌ [PAGAMENTO-{CorrelationId}] Caixa não encontrado: {CaixaId}", correlationId, caixaId);
                    throw new NotFoundException("Caixa não encontrado.");
                }

                return caixa;
            }

            private async Task<Domain.Entities.PlanoDeConta> ObterPlanoDeContas(string correlationId)
            {
                _logger.LogInformation("🔍 [PAGAMENTO-{CorrelationId}] Buscando plano de contas: Diarias", correlationId);
                var planoDeContas = await _unitOfWork.PlanoDeConta.GetByNameAsync("Diarias");

                if (planoDeContas == null)
                {
                    _logger.LogError("❌ [PAGAMENTO-{CorrelationId}] Plano de contas não encontrado: Diarias", correlationId);
                    throw new NotFoundException("Plano de contas 'Diarias' não encontrado.");
                }

                return planoDeContas;
            }

            private async Task<Domain.Entities.LancamentoCaixa> CriarLancamentoCaixa(
                CreatePagamentoCommand request, 
                int pagamentoId, 
                int caixaId, 
                string observacao,
                string correlationId)
            {
                try
                {
                    _logger.LogInformation("🏗️ [PAGAMENTO-{CorrelationId}] Criando lançamento de caixa", correlationId);
                    var planoDeContas = await ObterPlanoDeContas(correlationId);

                    var lancamentoCaixa = new Domain.Entities.LancamentoCaixa(
                        request.pagamentoRequest.Valor,
                        request.pagamentoRequest.DataPagamento,
                        request.pagamentoRequest.DataPagamento,
                        request.pagamentoRequest.TipoPagamentosId,
                        pagamentoId,
                        caixaId,
                        Domain.Enums.TipoLancamento.E, 
                        observacao, 
                        planoDeContas.Id, 
                        _logado.IdUtilizador);

                    lancamentoCaixa.DefinirValorPago(request.pagamentoRequest.ValorPago);
                    lancamentoCaixa.CalcularTroco();

                    _logger.LogInformation("✅ [PAGAMENTO-{CorrelationId}] Lançamento de caixa criado", correlationId);
                    return lancamentoCaixa;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 [PAGAMENTO-{CorrelationId}] Erro ao criar lançamento do caixa: {Message}", correlationId, ex.Message);
                    throw new InvalidOperationException($"Erro ao criar lançamento do caixa: {ex.Message}", ex);
                }
            }

            private BaseCommandResponse ResponseError(string message)
            {
                return new BaseCommandResponse
                {
                    Success = false,
                    Message = message
                };
            }
        }
    }

    // ✅ EXCEÇÕES CUSTOMIZADAS PARA MELHOR TRATAMENTO
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}