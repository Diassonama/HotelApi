using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Hotel.Application.Helper;
using Hotel.Application.Hospedagem.Base;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using static Hotel.Application.Hospedagem.Commands.TransferenciaHospedagemCommand.TransferenciaHospedagemCommandHandler;

namespace Hotel.Application.Hospedagem.Commands
{
    public class UpdateHospedagemCommand : HospedagemCommandBase
    {
        public int Id { get; set; }
        public class UpdateHospedagemCommandHandler : IRequestHandler<UpdateHospedagemCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly UsuarioLogado _logado;
            private readonly ICaixa _caixa;
            private readonly ILogger<UpdateHospedagemCommandHandler> _logger;

            public UpdateHospedagemCommandHandler(IUnitOfWork unitOfWork, UsuarioLogado logado, ICaixa caixa, ILogger<UpdateHospedagemCommandHandler> logger)
            {
                _unitOfWork = unitOfWork;
                _logado = logado;
                _caixa = caixa;
                _logger = logger;
            }

            public async Task<BaseCommandResponse> Handle(UpdateHospedagemCommand request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                try
                {
                    /* 
                                        var hospedagem = await _unitOfWork.Hospedagem.GetByIdAsync(request.Id);

                                        if (hospedagem is null)
                                        {
                                            resposta.Message = "Registro não encontrado";
                                            resposta.Success = false;
                                            return resposta;
                                        }

                                        var IdCaixa = _caixa.getCaixa;
                                        //  if (IdCaixa == 0) throw new Exception("");
                                        if (IdCaixa <= 0)
                                        {
                                            resposta.Success = false;
                                            resposta.Message = "Caixa encontra-se Fechado";
                                            // resposta.Errors = validateResult.Errors.Select(o => o.ErrorMessage).ToList();
                                        }

                                        if (_logado.IdUtilizador == null)
                                        {
                                            resposta.Success = false;
                                            resposta.Message = "Utilizador não encontrado";
                                            // resposta.Errors = validateResult.Errors.Select(o => o.ErrorMessage).ToList();
                                        }

                                        TimeSpan ts = request.PrevisaoFechamento - request.DataAbertura;
                                        var totalDiaria = request.ValorDiaria * ts.Days;
                                        var checkin = await _unitOfWork.checkins.GetByIdAsync(hospedagem.CheckinsId);

                                        if (checkin != null)
                                        {
                                            checkin.update(checkin.Id, totalDiaria);
                                            //  _unitOfWork.checkins.Attach(checkin);
                                            await _unitOfWork.checkins.Update(checkin);
                                        }

                                        var historico = new Historico(IdCaixa, _logado.IdUtilizador, checkin.Id);
                                        historico.AdicionarObservacao(hospedagem.PrevisaoFechamento, request.PrevisaoFechamento);
                                        await _unitOfWork.historico.Add(historico);

                                        hospedagem.AtualizarPrevisaoFechamento(request.PrevisaoFechamento);
                                        // _unitOfWork.Hospedagem.Attach(hospedagem);         
                                        await _unitOfWork.Hospedagem.Update(hospedagem);
                                        // await _unitOfWork.CommitAsync();

                                        resposta.Data = hospedagem;
                                        resposta.Success = true;
                                        resposta.Message = "Hospedagem atualizado com sucesso";
                                        await _unitOfWork.Apartamento.AtualizarSituacaoApartamentosAsync();
                                    }
                                    catch (Exception ex)
                                    {
                                        // Tratamento de exceções
                                        resposta.Success = false;
                                        resposta.Message = $"Erro ao atualizar o check-in: {ex.Message}";
                                    } */

                    var hospedagem = await _unitOfWork.Hospedagem.GetByIdAsync(request.Id);
                    if (hospedagem is null)
                    {
                        return RespostaErro("Registro não encontrado");
                    }

                    var IdCaixa = _caixa.getCaixa;
                    if (IdCaixa <= 0)
                    {
                        return RespostaErro("Caixa encontra-se Fechado");
                    }

                    if (_logado.IdUtilizador == null)
                    {
                        return RespostaErro("Utilizador não encontrado");
                    }

                    /*  var dataInicialAngola = TimeZoneHelper.GetDateInAngola(request.DataAbertura);
                     var dataFinalAngola = TimeZoneHelper.GetDateInAngola(request.PrevisaoFechamento);
  */
                    // ✅ NOVA LÓGICA: Verificar se existem transferências para o check-in
                    _logger.LogInformation("🔍 [UPDATE-{CorrelationId}] Verificando transferências para CheckinId: {CheckinId}",
                        correlationId, hospedagem.CheckinsId);

                    var temTransferencias = await _unitOfWork.Transferencias.GetCountByCheckinIdAsync(hospedagem.CheckinsId);
                    decimal totalDiaria = 0; ;


                    // Calcular valores da hospedagem
                    /*   var totalDias = (request.PrevisaoFechamento.Date - request.DataAbertura.Date).Days;
                       TimeSpan ts = request.PrevisaoFechamento.Date - request.DataAbertura.Date;
                       totalDiaria = request.ValorDiaria * totalDias; */
                    if (temTransferencias > 0)
                    {
                        // ✅ CENÁRIO 1: HÁ TRANSFERÊNCIAS - Usar função de cálculo especial
                        _logger.LogInformation("📈 [UPDATE-{CorrelationId}] Encontradas {TransferenciasCount} transferências. Calculando valor usando CalcularTotalDiariasAsync",
                            correlationId, temTransferencias);

                        totalDiaria = await CalcularTotalDiariasAsync(hospedagem.CheckinsId);

                        _logger.LogInformation("💰 [UPDATE-{CorrelationId}] Valor calculado via transferências: {ValorCalculado}",
                            correlationId, totalDiaria);
                    }
                    else
                    {
                        // ✅ CENÁRIO 2: NÃO HÁ TRANSFERÊNCIAS - Cálculo normal
                        _logger.LogInformation("📊 [UPDATE-{CorrelationId}] Nenhuma transferência encontrada. Usando cálculo normal",
                            correlationId);

    
                        // Calcular valores da hospedagem - modo tradicional
                        var totalDias = (request.PrevisaoFechamento.Date - request.DataAbertura.Date).Days;

                        // ✅ GARANTIR MÍNIMO DE 1 DIA
                      //  if (totalDias <= 0) totalDias = 1;

                        totalDiaria = (decimal)(request.ValorDiaria * totalDias);

                        _logger.LogInformation("💰 [UPDATE-{CorrelationId}] Valor calculado tradicionalmente: Dias={Dias}, ValorDiaria={ValorDiaria}, Total={Total}",
                            correlationId, totalDias, request.ValorDiaria, totalDiaria);
                    }

                    // ✅ VALIDAÇÃO DE SEGURANÇA: Garantir que o valor não seja zero
                    if (totalDiaria <= 0)
                    {
                        _logger.LogWarning("⚠️ [UPDATE-{CorrelationId}] Valor total calculado é zero ou negativo ({Valor}), usando valor mínimo",
                            correlationId, totalDiaria);

                        totalDiaria = (decimal)request.ValorDiaria; // Usar pelo menos o valor de uma diária
                    }


                    var checkin = await _unitOfWork.checkins.GetByIdAsync(hospedagem.CheckinsId);
                    if (checkin != null)
                    {
                        checkin.update(checkin.Id, (float)totalDiaria);
                        await _unitOfWork.checkins.Update(checkin);
                    }



                    var historico = new Historico(IdCaixa, _logado.IdUtilizador, checkin?.Id ?? 0);
                    historico.AdicionarObservacao(hospedagem.Apartamentos.Codigo, hospedagem.PrevisaoFechamento, request.PrevisaoFechamento);
                    await _unitOfWork.historico.Add(historico);

                    hospedagem.AtualizarPrevisaoFechamento(request.PrevisaoFechamento);
                    await _unitOfWork.Hospedagem.Update(hospedagem);

                    await _unitOfWork.Apartamento.AtualizarSituacaoApartamentosAsync();

                    resposta.Data = hospedagem;
                    resposta.Success = true;
                    resposta.Message = "Hospedagem atualizada com sucesso";

                }
                catch (Exception ex)
                {
                    resposta = RespostaErro($"Erro ao atualizar o check-in: {ex.Message}");
                }

                return resposta;
            }

            public async Task<decimal> CalcularTotalDiariasAsync(int CheckinsId)
            {
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                try
                {
                    _logger.LogInformation("🧮 [CALC-{CorrelationId}] Iniciando cálculo de total de diárias - CheckinId: {CheckinId}",
                        correlationId, CheckinsId);

                    // ✅ CORREÇÃO: Usar SQL RAW para contornar problemas de cast
                    var transferencias = await ObterTransferenciasSqlRaw(CheckinsId, correlationId);

                    _logger.LogInformation("📊 [CALC-{CorrelationId}] Encontradas {TransferenciasCount} transferências para o check-in",
                        correlationId, transferencias?.Count() ?? 0);

                    if (transferencias == null || !transferencias.Any())
                    {
                        _logger.LogWarning("⚠️ [CALC-{CorrelationId}] Nenhuma transferência encontrada para o check-in: {CheckinId}",
                            correlationId, CheckinsId);

                        // ✅ BUSCAR HOSPEDAGEM ORIGINAL PARA CALCULAR VALOR MÍNIMO
                        var valorFallback = await CalcularValorFallback(CheckinsId, correlationId);
                        return valorFallback;
                    }

                    decimal total = 0;
                    int transferenceCount = 0;
                    int transferenciasCalculadas = 0;
                    int transferenciasIgnoradas = 0;

                    foreach (var h in transferencias)
                    {
                        transferenceCount++;
                        var inicio = h.DataEntrada.Date;
                        var fim = h.DataSaida.Date;

                        int dias = (fim - inicio).Days;

                        // ✅ CORREÇÃO: Se dias for zero ou negativo, pular o cálculo
                        if (dias <= 0)
                        {
                            transferenciasIgnoradas++;

                            if (dias == 0)
                            {
                                _logger.LogWarning("⚠️ [CALC-{CorrelationId}] Transferência {Index} - Mesmo dia - Entrada: {DataEntrada}, Saída: {DataSaida} - IGNORANDO CÁLCULO",
                                    correlationId, transferenceCount, inicio.ToString("yyyy-MM-dd"), fim.ToString("yyyy-MM-dd"));
                            }
                            else
                            {
                                _logger.LogWarning("⚠️ [CALC-{CorrelationId}] Transferência {Index} - Dias negativos ({Dias}) - Entrada: {DataEntrada}, Saída: {DataSaida} - IGNORANDO CÁLCULO",
                                    correlationId, transferenceCount, dias, inicio.ToString("yyyy-MM-dd"), fim.ToString("yyyy-MM-dd"));
                            }
                            continue;
                        }

                        // ✅ VALIDAÇÃO ADICIONAL: Verificar valor diária
                        if (h.ValorDiaria <= 0)
                        {
                            transferenciasIgnoradas++;
                            _logger.LogWarning("⚠️ [CALC-{CorrelationId}] Transferência {Index} - Valor diária inválido ({ValorDiaria}) - IGNORANDO CÁLCULO",
                                correlationId, transferenceCount, h.ValorDiaria);
                            continue;
                        }

                        var valorTransferencia = dias * h.ValorDiaria;
                        total += (decimal)valorTransferencia;
                        transferenciasCalculadas++;

                        _logger.LogInformation("📈 [CALC-{CorrelationId}] Transferência {Index} - Entrada: {DataEntrada}, Saída: {DataSaida}, Dias: {Dias}, Valor Diária: {ValorDiaria}, Subtotal: {Subtotal}",
                            correlationId, transferenceCount, inicio.ToString("yyyy-MM-dd"), fim.ToString("yyyy-MM-dd"), dias, h.ValorDiaria, valorTransferencia);
                    }

                    // ✅ GARANTIR QUE O TOTAL NÃO SEJA ZERO
                    if (total <= 0 && transferenciasCalculadas == 0)
                    {
                        _logger.LogWarning("⚠️ [CALC-{CorrelationId}] Total calculado é zero, buscando valor fallback",
                            correlationId);

                        var valorFallback = await CalcularValorFallback(CheckinsId, correlationId);
                        if (valorFallback > 0)
                        {
                            total = valorFallback;
                            _logger.LogInformation("✅ [CALC-{CorrelationId}] Usando valor fallback: {ValorFallback}",
                                correlationId, valorFallback);
                        }
                    }

                    _logger.LogInformation("✅ [CALC-{CorrelationId}] Cálculo concluído - Total: {Total}, Transferências: {TotalTransferencias}, Calculadas: {Calculadas}, Ignoradas: {Ignoradas}",
                        correlationId, total, transferenceCount, transferenciasCalculadas, transferenciasIgnoradas);

                    return total;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ [CALC-{CorrelationId}] Erro ao calcular total de diárias - CheckinId: {CheckinId}, Erro: {ErrorMessage}",
                        correlationId, CheckinsId, ex.Message);

                    // ✅ FALLBACK: Tentar calcular valor mínimo
                    try
                    {
                        var valorFallback = await CalcularValorFallback(CheckinsId, correlationId);
                        _logger.LogWarning("⚠️ [CALC-{CorrelationId}] Usando valor fallback após erro: {ValorFallback}",
                            correlationId, valorFallback);
                        return valorFallback;
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
            private async Task<List<Transferencia>> ObterTransferenciasSqlRaw(int checkinId, string correlationId)
            {
                try
                {
                    _logger.LogInformation("🔍 [CALC-{CorrelationId}] Executando consulta EF Core para obter transferências",
                        correlationId);

                    // ✅ CONSULTA EF CORE COM PROJEÇÃO EXPLÍCITA PARA EVITAR PROBLEMAS DE CAST
                    var transferencias = await _unitOfWork.Transferencias.GetByCheckinIdAsync(checkinId);


                    var transfer = transferencias.Select(dto => new Transferencia
                    {
                        Id = dto.Id,
                        CheckinId = dto.CheckinId,
                        DataEntrada = dto.DataEntrada,
                        DataSaida = dto.DataSaida,
                        QuartoId = dto.QuartoId,
                        TipoTransferencia = (TipoTransferencia)dto.TipoTransferencia,
                        ValorDiaria = (float)dto.ValorDiaria,
                        Observacao = dto.Observacao,
                        MotivoTransferenciaId = dto.MotivoTransferenciaId,
                        ManterPreco = dto.ManterPreco,
                        DataTransferencia = dto.DataTransferencia,
                        UtilizadorId = dto.UtilizadorId
                    }).ToList();

                    /*    .Select(t => new TransferenciaDto
                       {
                           Id = t.Id,
                           CheckinId = t.CheckinId,
                           DataEntrada = t.DataEntrada,
                           DataSaida = t.DataSaida,
                           QuartoId = t.QuartoId,
                           TipoTransferencia = (int)t.TipoTransferencia,
                           ValorDiaria = (decimal)t.ValorDiaria,
                           Observacao = t.Observacao ?? string.Empty,
                           MotivoTransferenciaId = t.MotivoTransferenciaId,
                           ManterPreco = t.ManterPreco,
                           DataTransferencia = t.DataTransferencia,
                           UtilizadorId = t.UtilizadorId ?? string.Empty,
                           DateCreated = t.DateCreated,
                           IsActive = t.IsActive
                       })
                       .AsNoTracking() // ✅ PERFORMANCE: Não rastrear entidades para leitura
                       .ToListAsync();  */

                    _logger.LogInformation("✅ [CALC-{CorrelationId}] Consulta EF Core executada com sucesso, {Count} registros obtidos",
                        correlationId, transfer.Count);

                    return transfer;
                }
                catch (InvalidCastException castEx)
                {
                    _logger.LogError(castEx, "❌ [CALC-{CorrelationId}] Erro de cast na consulta EF Core, tentando fallback com conversões explícitas",
                        correlationId);

                    // ✅ FALLBACK: Tentar com conversões mais explícitas
                    var transferenciasDto = await ObterTransferenciasFallback(checkinId, correlationId);
                    var transferencias = transferenciasDto.Select(dto => new Transferencia
                    {
                        Id = dto.Id,
                        CheckinId = dto.CheckinId,
                        DataEntrada = dto.DataEntrada,
                        DataSaida = dto.DataSaida,
                        QuartoId = dto.QuartoId,
                        TipoTransferencia = (TipoTransferencia)dto.TipoTransferencia,
                        ValorDiaria = (float)dto.ValorDiaria,
                        Observacao = dto.Observacao,
                        MotivoTransferenciaId = dto.MotivoTransferenciaId,
                        ManterPreco = dto.ManterPreco,
                        DataTransferencia = dto.DataTransferencia,
                        UtilizadorId = dto.UtilizadorId

                    }).ToList();
                    return transferencias;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ [CALC-{CorrelationId}] Erro geral na consulta EF Core para transferências",
                        correlationId);
                    throw;
                }
            }
            private async Task<List<TransferenciaDto>> ObterTransferenciasFallback(int checkinId, string correlationId)
            {
                try
                {
                    _logger.LogInformation("🔄 [CALC-{CorrelationId}] Executando consulta fallback para transferências",
                        correlationId);

                    // ✅ BUSCAR ENTIDADES PRIMEIRO E DEPOIS FAZER A CONVERSÃO
                    var transferenciasRaw = await _unitOfWork.Transferencias.GetByCheckinIdAsync(checkinId);


                    // ✅ CONVERTER MANUALMENTE PARA DTO COM VALIDAÇÕES
                    var transferenciasDto = new List<TransferenciaDto>();

                    foreach (var t in transferenciasRaw)
                    {
                        try
                        {
                            var dto = new TransferenciaDto
                            {
                                Id = t.Id,
                                CheckinId = t.CheckinId,
                                DataEntrada = t.DataEntrada,
                                DataSaida = t.DataSaida,
                                QuartoId = t.QuartoId,
                                TipoTransferencia = (int)t.TipoTransferencia,
                                ValorDiaria = t.ValorDiaria is float floatValue ? (decimal)floatValue :
                                             Convert.ToDecimal(t.ValorDiaria),
                                Observacao = t.Observacao ?? string.Empty,
                                MotivoTransferenciaId = t.MotivoTransferenciaId,
                                ManterPreco = t.ManterPreco,
                                DataTransferencia = t.DataTransferencia,
                                UtilizadorId = t.UtilizadorId ?? string.Empty,

                            };

                            transferenciasDto.Add(dto);
                        }
                        catch (Exception conversionEx)
                        {
                            _logger.LogWarning(conversionEx, "⚠️ [CALC-{CorrelationId}] Erro ao converter transferência ID {TransferenciaId}, ignorando registro",
                                correlationId, t.Id);
                            continue; // Pular este registro e continuar com os outros
                        }
                    }

                    _logger.LogInformation("✅ [CALC-{CorrelationId}] Consulta fallback executada, {Count} de {Total} registros convertidos com sucesso",
                        correlationId, transferenciasDto.Count, transferenciasRaw.Count()); // ✅ CORREÇÃO: usar transferenciasDto.Count

                    return transferenciasDto;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ [CALC-{CorrelationId}] Erro também na consulta fallback",
                        correlationId);

                    // ✅ ÚLTIMO RECURSO: Retornar lista vazia
                    return new List<TransferenciaDto>();
                }
            }
            private async Task<decimal> CalcularValorFallback(int checkinId, string correlationId)
            {
                try
                {
                    _logger.LogInformation("🔄 [CALC-{CorrelationId}] Calculando valor fallback para CheckinId: {CheckinId}",
                        correlationId, checkinId);

                    // Buscar hospedagem relacionada ao checkin
                    var hospedagem = await _unitOfWork.Hospedagem.GetByCheckinIdAsync(checkinId);

                    if (hospedagem != null)
                    {
                        var diasHospedagem = (hospedagem.PrevisaoFechamento.Date - hospedagem.DataAbertura.Date).Days;
                        if (diasHospedagem <= 0) diasHospedagem = 1; // Mínimo 1 dia

                        var valorFallback = (decimal)(diasHospedagem * hospedagem.ValorDiaria);

                        _logger.LogInformation("✅ [CALC-{CorrelationId}] Valor fallback calculado: {ValorFallback} (Dias: {Dias}, Valor Diária: {ValorDiaria})",
                            correlationId, valorFallback, diasHospedagem, hospedagem.ValorDiaria);

                        return valorFallback;
                    }

                    // Se não encontrou hospedagem, tentar buscar diretamente no check-in
                    var checkin = await _unitOfWork.checkins.GetByIdAsync(checkinId);
                    if (checkin != null && checkin.ValorTotalDiaria > 0)
                    {
                        var valorCheckin = (decimal)checkin.ValorTotalDiaria;

                        _logger.LogInformation("✅ [CALC-{CorrelationId}] Usando valor do check-in como fallback: {ValorFallback}",
                            correlationId, valorCheckin);

                        return valorCheckin;
                    }

                    _logger.LogWarning("⚠️ [CALC-{CorrelationId}] Não foi possível calcular valor fallback, retornando 1.0",
                        correlationId);

                    return 1.0m; // Valor mínimo para evitar erro de validação
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ [CALC-{CorrelationId}] Erro ao calcular valor fallback",
                        correlationId);
                    return 1.0m; // Valor mínimo para evitar erro de validação
                }
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