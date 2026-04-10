using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Checkin.Commands
{
    public class CheckOutCommand : IRequest<BaseCommandResponse>
    {
        public int CheckinsId { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int HospedesId { get; set; }

        public class CheckOutCommandHandled : IRequestHandler<CheckOutCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly UsuarioLogado _logado;
            private readonly ICaixa _caixa;
            private readonly IRackNotificationService _rackNotificationService;
            private readonly ILogger<CheckOutCommandHandled> _logger;

            public CheckOutCommandHandled(
                IUnitOfWork unitOfWork,
                UsuarioLogado logado,
                ICaixa caixa,
                IRackNotificationService rackNotificationService,
                ILogger<CheckOutCommandHandled> logger)
            {
                _unitOfWork = unitOfWork;
                _logado = logado;
                _caixa = caixa;
                _rackNotificationService = rackNotificationService;
                _logger = logger;
            }

            public async Task<BaseCommandResponse> Handle(CheckOutCommand request, CancellationToken cancellationToken)
            {
                BaseCommandResponse response = new BaseCommandResponse();
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                try
                {
                    _logger.LogInformation("🏨 [CHECKOUT-{CorrelationId}] Iniciando processo de check-out - CheckinId: {CheckinId}, HospedeId: {HospedeId}, DataCheckout: {DataCheckout}, Usuario: {Usuario}",
                        correlationId, request.CheckinsId, request.HospedesId, request.CheckOutDate, _logado.IdUtilizador);

                    // ✅ VALIDAÇÃO DO CAIXA
                    if (_caixa.getCaixa == 0)
                    {
                        _logger.LogWarning("⚠️ [CHECKOUT-{CorrelationId}] Tentativa de checkout com caixa fechado - Usuario: {Usuario}",
                            correlationId, _logado.IdUtilizador);

                        response.Success = false;
                        response.Message = "Erro o caixa esta fechado queira por favor abrir";
                        return response;
                    }

                    _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Caixa validado - CaixaId: {CaixaId}",
                        correlationId, _caixa.getCaixa);

                    // ✅ BUSCAR CONFIGURAÇÃO FISCAL
                    _logger.LogInformation("🔍 [CHECKOUT-{CorrelationId}] Buscando configuração fiscal",
                        correlationId);

                    var configuracaoFiscal = await _unitOfWork.Param.Get(1);
                    if (configuracaoFiscal == null)
                    {
                        _logger.LogError("❌ [CHECKOUT-{CorrelationId}] Configuração fiscal não encontrada",
                            correlationId);
                        throw new ArgumentException("Configuração do sistema não encontrada.");
                    }

                    _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Configuração fiscal encontrada - Ano: {Ano}",
                        correlationId, configuracaoFiscal.DataInicio.Year);

                    // ✅ BUSCAR HOSPEDAGEM
                    _logger.LogInformation("🔍 [CHECKOUT-{CorrelationId}] Buscando hospedagem - CheckinId: {CheckinId}",
                        correlationId, request.CheckinsId);

                    var hospedagem = await _unitOfWork.Hospedagem.GetByCheckinIdAsync(request.CheckinsId);
                    if (hospedagem == null)
                    {
                        _logger.LogError("❌ [CHECKOUT-{CorrelationId}] Hospedagem não encontrada - CheckinId: {CheckinId}",
                            correlationId, request.CheckinsId);
                        throw new ArgumentException("Hospedagem não encontrada.");
                    }

                    _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Hospedagem encontrada - HospedagemId: {HospedagemId}, ApartamentoId: {ApartamentoId}, EmpresaId: {EmpresaId}",
                        correlationId, hospedagem.Id, hospedagem.ApartamentosId, hospedagem.EmpresasId);

                    // ✅ BUSCAR APARTAMENTO
                    _logger.LogInformation("🔍 [CHECKOUT-{CorrelationId}] Buscando apartamento - ApartamentoId: {ApartamentoId}",
                        correlationId, hospedagem.ApartamentosId);

                    var apartamento = await _unitOfWork.Apartamento.GetByIdAsync(hospedagem.ApartamentosId);
                    if (apartamento == null)
                    {
                        _logger.LogError("❌ [CHECKOUT-{CorrelationId}] Apartamento não encontrado - ApartamentoId: {ApartamentoId}",
                            correlationId, hospedagem.ApartamentosId);
                        throw new Exception("Apartamento não encontrado.");
                    }

                    _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Apartamento encontrado - Codigo: {Codigo}, Status: {Status}",
                        correlationId, apartamento.Codigo, apartamento.IsActive);

                    // ✅ BUSCAR CHECK-IN
                    _logger.LogInformation("🔍 [CHECKOUT-{CorrelationId}] Buscando check-in - CheckinId: {CheckinId}",
                        correlationId, request.CheckinsId);

                    var checkin = await _unitOfWork.checkins.GetByIdAsync(request.CheckinsId);
                    if (checkin == null)
                    {
                        _logger.LogError("❌ [CHECKOUT-{CorrelationId}] Check-in não encontrado - CheckinId: {CheckinId}",
                            correlationId, request.CheckinsId);
                        throw new ArgumentException("Check-in não encontrado.");
                    }

                    _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Check-in encontrado - ValorTotal: {ValorTotal}, SituacaoPagamento: {SituacaoPagamento}",
                        correlationId, checkin.ValorTotalFinal, checkin.situacaoDoPagamento);

                    // ✅ BUSCAR HÓSPEDE
                    _logger.LogInformation("🔍 [CHECKOUT-{CorrelationId}] Buscando hóspede - HospedeId: {HospedeId}",
                        correlationId, request.HospedesId);

                    var hospede = await _unitOfWork.hospedes.GetByIdAsync(request.HospedesId);
                    if (hospede == null)
                    {
                        _logger.LogError("❌ [CHECKOUT-{CorrelationId}] Hóspede não encontrado - HospedeId: {HospedeId}",
                            correlationId, request.HospedesId);
                        throw new ArgumentException("Hospedagem não encontrado.");
                    }

                    _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Hóspede encontrado - Nome: {Nome}, Estado: {Estado}",
                        correlationId, hospede.Clientes.Nome, hospede.Estado);

                    // ✅ VALIDAR PAGAMENTO
                    _logger.LogInformation("🔍 [CHECKOUT-{CorrelationId}] Validando situação do pagamento - SituacaoPagamento: {SituacaoPagamento}",
                        correlationId, checkin.situacaoDoPagamento);

                    if (!hospede.PodeFazerCheckout(checkin.situacaoDoPagamento))
                    {
                        _logger.LogWarning("⚠️ [CHECKOUT-{CorrelationId}] Hóspede não pode fazer checkout sem pagamento - HospedeId: {HospedeId}, SituacaoPagamento: {SituacaoPagamento}",
                            correlationId, request.HospedesId, checkin.situacaoDoPagamento);
                        throw new ArgumentException("O hóspede não pode fazer o checkout sem pagar.");
                    }

                    _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Validação de pagamento aprovada",
                        correlationId);

                    // ✅ ATUALIZAR CHECK-IN
                    _logger.LogInformation("🔧 [CHECKOUT-{CorrelationId}] Atualizando dados do check-in",
                        correlationId);

                    checkin.UtilizadoECaixaCheckOut(_caixa.getCaixa, _logado.IdUtilizador);
                    checkin.ValidarCheckout(hospedagem.PrevisaoFechamento, request.CheckOutDate);

                    _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Check-in atualizado - CaixaCheckout: {CaixaCheckout}, UsuarioCheckout: {UsuarioCheckout}",
                        correlationId, _caixa.getCaixa, _logado.IdUtilizador);

                    // ✅ FECHAR HOSPEDAGEM
                    _logger.LogInformation("🔧 [CHECKOUT-{CorrelationId}] Fechando hospedagem",
                        correlationId);

                    hospedagem.FecharHospedagem(request.CheckOutDate);

                    _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Hospedagem fechada - DataSaida: {DataSaida}",
                        correlationId, request.CheckOutDate);

                    // ✅ PROCESSAR FATURAÇÃO PARA EMPRESA
                    if (hospede.Estado == Hospede.EstadoHospede.Empresa)
                    {
                        _logger.LogInformation("🏢 [CHECKOUT-{CorrelationId}] Processando faturação para empresa - EmpresaId: {EmpresaId}",
                            correlationId, hospedagem.EmpresasId);

                        if (checkin.situacaoDoPagamento == Checkins.SituacaoDoPagamento.Pendente) // Ajustado para string
                        {
                            _logger.LogInformation("📄 [CHECKOUT-{CorrelationId}] Criando fatura empresa - Série: FR, Ano: {Ano}",
                                correlationId, configuracaoFiscal.DataInicio.Year);

                            int numeroFactura = await _unitOfWork.series.NumeradorAsync("FR", configuracaoFiscal.DataInicio.Year);

                            _logger.LogInformation("🔢 [CHECKOUT-{CorrelationId}] Número da fatura gerado - NumeroFactura: {NumeroFactura}",
                                correlationId, numeroFactura);

                            var facturaEmpresa = new Domain.Entities.FacturaEmpresa(
                                checkin.Id,
                                numeroFactura,
                                checkin.ValorTotalDiaria,
                                hospedagem.EmpresasId,
                                request.CheckOutDate,
                                "FR",
                                configuracaoFiscal.DataInicio.Year);

                            /*  await _unitOfWork.Factura.Add(facturaEmpresa);

                             var conta = new ContaReceber(hospedagem.EmpresasId, (decimal)checkin.ValorTotalDiaria, DateTime.Now, DateTime.Now.AddDays(7), numeroFactura.ToString(), checkin.Id, "Fatura Empresa Checkout FR - " + numeroFactura);
                             await _unitOfWork.ContasReceber.Add(conta);
                             await _unitOfWork.Save(); */

                            _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Fatura empresa criada - FacturaId: {FacturaId}, Numero: {Numero}, Valor: {Valor}",
                                correlationId, facturaEmpresa.Id, numeroFactura, checkin.ValorTotalDiaria);


                            // Criar Conta a Receber com documento formatado
                            var documentoFactura = $"FR-{numeroFactura}/{configuracaoFiscal.DataInicio.Year}";
                            var dataVencimento = DateTime.Now.AddDays(7); // Vencimento padrão: 7 dias
                            var observacaoFactura = $"Fatura Empresa Checkout - FR {numeroFactura} - Apartamento {apartamento.Codigo}";

                            var contaReceber = new ContaReceber(
                                empresaId: hospedagem.EmpresasId,
                                valorTotal: (decimal)checkin.ValorTotalDiaria,
                                dataEmissao: DateTime.Now,
                                vencimento: dataVencimento,
                                documento: documentoFactura,
                                checkinsId: checkin.Id,
                                observacao: observacaoFactura);

                            await _unitOfWork.ContasReceber.Add(contaReceber);
                            await _unitOfWork.Save();

                            _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Fatura e conta a receber criadas - FacturaId: {FacturaId}, ContaId: {ContaId}, Numero: {Numero}, Valor: {Valor}, Vencimento: {Vencimento}",
                                correlationId, facturaEmpresa.Id, contaReceber.Id, numeroFactura, checkin.ValorTotalDiaria, dataVencimento);

                        }
                        else
                        {
                            _logger.LogInformation("ℹ️ [CHECKOUT-{CorrelationId}] Fatura não criada - Pagamento já realizado",
                                correlationId);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("ℹ️ [CHECKOUT-{CorrelationId}] Hóspede não é empresa - não há faturação",
                            correlationId);
                    }

                    // ✅ LIBERAR APARTAMENTO
                    _logger.LogInformation("🔓 [CHECKOUT-{CorrelationId}] Liberando apartamento - ApartamentoId: {ApartamentoId}",
                        correlationId, apartamento.Id);

                    apartamento.liberarApartamento();

                    _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Apartamento liberado - Codigo: {Codigo}",
                        correlationId, apartamento.Codigo);

                    // ✅ SALVAR ALTERAÇÕES
                    _logger.LogInformation("💾 [CHECKOUT-{CorrelationId}] Salvando alterações no banco de dados",
                        correlationId);

                    await _unitOfWork.checkins.Update(checkin);

                    // Notificações em tempo real do rack
                    await _rackNotificationService.NotifyCheckoutAsync(checkin);
                    await _rackNotificationService.NotifyApartmentStatusChangeAsync(apartamento);
                    await _rackNotificationService.NotifyRackUpdateAsync();

                    _logger.LogInformation("✅ [CHECKOUT-{CorrelationId}] Check-out realizado com sucesso - CheckinId: {CheckinId}, ApartamentoCodigo: {ApartamentoCodigo}",
                        correlationId, request.CheckinsId, apartamento.Codigo);

                    response.Success = true;
                    response.Message = "checkOut realizado com sucesso";
                }
                catch (ArgumentException argEx)
                {
                    _logger.LogWarning("⚠️ [CHECKOUT-{CorrelationId}] Erro de validação: {Message}",
                        correlationId, argEx.Message);

                    response.Success = false;
                    response.Message = argEx.Message;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ [CHECKOUT-{CorrelationId}] Erro interno ao realizar check-out - CheckinId: {CheckinId}, HospedeId: {HospedeId}: {Message}",
                        correlationId, request.CheckinsId, request.HospedesId, ex.Message);

                    response.Success = false;
                    response.Message = $"Erro ao realizar o check-out: {ex.Message}";
                }

                return response;
            }
        }
    }
}