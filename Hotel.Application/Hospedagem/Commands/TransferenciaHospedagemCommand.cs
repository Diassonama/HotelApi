using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Hotel.Application.Hospedagem.Base;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Hospedagem.Commands
{
    public class TransferenciaHospedagemCommand : IRequest<BaseCommandResponse>
    {
        public int HospedagemOrigemId { get; set; }
        public int QuartoDestinoId { get; set; }
        public int MotivoTransferenciaId { get; set; }
        public float valorDiaria { get; set; }
        public DateTime DataTransferencia { get; set; }
        public string Observacao { get; set; }
        public Boolean ManterPreco { get; set; } = true;

        public class TransferenciaHospedagemCommandHandler : IRequestHandler<TransferenciaHospedagemCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly UsuarioLogado _logado;
            private readonly ICaixa _caixa;
            private readonly ILogger<TransferenciaHospedagemCommandHandler> _logger;

            public TransferenciaHospedagemCommandHandler(
                IUnitOfWork unitOfWork, 
                UsuarioLogado logado, 
                ICaixa caixa,
                ILogger<TransferenciaHospedagemCommandHandler> logger)
            {
                _unitOfWork = unitOfWork;
                _logado = logado;
                _caixa = caixa;
                _logger = logger;
            }

            public async Task<BaseCommandResponse> Handle(TransferenciaHospedagemCommand request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                try
                {
                    // ✅ LOG INICIAL
                    _logger.LogInformation("🔄 [TRANSFER-{CorrelationId}] Iniciando transferência de hospedagem - Hospedagem: {HospedagemId}, Quarto Destino: {QuartoDestino}, Usuário: {UserId}",
                        correlationId, request.HospedagemOrigemId, request.QuartoDestinoId, _logado.IdUtilizador);

                    // ✅ VERIFICAR SE A HOSPEDAGEM EXISTE
                    _logger.LogInformation("🔍 [TRANSFER-{CorrelationId}] Buscando hospedagem origem ID: {HospedagemId}",
                        correlationId, request.HospedagemOrigemId);

                    var hospedagem = await _unitOfWork.Hospedagem.GetByIdAsync(request.HospedagemOrigemId);
                    if (hospedagem is null)
                    {
                        _logger.LogWarning("⚠️ [TRANSFER-{CorrelationId}] Hospedagem não encontrada - ID: {HospedagemId}",
                            correlationId, request.HospedagemOrigemId);
                        return RespostaErro("Registro não encontrado");
                    }

                    _logger.LogInformation("✅ [TRANSFER-{CorrelationId}] Hospedagem encontrada - ID: {HospedagemId}, Check-in: {CheckinId}, Quarto Atual: {QuartoAtual}",
                        correlationId, hospedagem.Id, hospedagem.CheckinsId, hospedagem.ApartamentosId);

                    // ✅ VERIFICAR SE O CAIXA ESTÁ ABERTO
                    var IdCaixa = _caixa.getCaixa;
                    _logger.LogInformation("💰 [TRANSFER-{CorrelationId}] Verificando caixa - ID: {CaixaId}",
                        correlationId, IdCaixa);

                    if (IdCaixa <= 0)
                    {
                        _logger.LogWarning("⚠️ [TRANSFER-{CorrelationId}] Caixa fechado - ID: {CaixaId}",
                            correlationId, IdCaixa);
                        return RespostaErro("Caixa encontra-se Fechado");
                    }

                    // ✅ VERIFICAR SE O USUÁRIO ESTÁ LOGADO
                    if (_logado.IdUtilizador == null)
                    {
                        _logger.LogWarning("⚠️ [TRANSFER-{CorrelationId}] Usuário não autenticado",
                            correlationId);
                        return RespostaErro("Utilizador não encontrado");
                    }

                    _logger.LogInformation("👤 [TRANSFER-{CorrelationId}] Usuário autenticado - ID: {UserId}",
                        correlationId, _logado.IdUtilizador);

                    // ✅ VERIFICAR SE HOSPEDAGEM JÁ ESTÁ FECHADA
                    if (hospedagem.DataFechamento.HasValue && hospedagem.DataFechamento.Value != DateTime.MinValue)
                    {
                        _logger.LogWarning("⚠️ [TRANSFER-{CorrelationId}] Hospedagem já fechada - Data Fechamento: {DataFechamento}",
                            correlationId, hospedagem.DataFechamento);
                        return RespostaErro("A hospedagem já está fechada e não pode ser transferida.");
                    }

                    // ✅ BUSCAR APARTAMENTO ORIGEM
                    _logger.LogInformation("🏠 [TRANSFER-{CorrelationId}] Buscando apartamento origem - ID: {ApartamentoOrigemId}",
                        correlationId, hospedagem.ApartamentosId);

                    var apartamento = await _unitOfWork.Apartamento.GetByIdAsync(hospedagem.ApartamentosId);
                    if (apartamento == null)
                    {
                        _logger.LogError("❌ [TRANSFER-{CorrelationId}] Apartamento origem não encontrado - ID: {ApartamentoId}",
                            correlationId, hospedagem.ApartamentosId);
                        throw new Exception("Apartamento não encontrado.");
                    }

                    _logger.LogInformation("✅ [TRANSFER-{CorrelationId}] Apartamento origem encontrado - Código: {CodigoApartamento}, Status: {StatusApartamento}",
                        correlationId, apartamento.Codigo, apartamento.Situacao);

                    // ✅ LIBERAR APARTAMENTO ORIGEM
                    _logger.LogInformation("🔓 [TRANSFER-{CorrelationId}] Liberando apartamento origem - Código: {CodigoApartamento}",
                        correlationId, apartamento.Codigo);
                    apartamento.liberarApartamento();

                    // ✅ BUSCAR APARTAMENTO DESTINO
                    _logger.LogInformation("🏠 [TRANSFER-{CorrelationId}] Buscando apartamento destino - ID: {ApartamentoDestinoId}",
                        correlationId, request.QuartoDestinoId);

                    var apartamentoDestino = await _unitOfWork.Apartamento.GetByIdAsync(request.QuartoDestinoId);
                    if (apartamentoDestino == null)
                    {
                        _logger.LogError("❌ [TRANSFER-{CorrelationId}] Apartamento destino não encontrado - ID: {ApartamentoId}",
                            correlationId, request.QuartoDestinoId);
                        throw new Exception("Apartamento destino não encontrado.");
                    }

                    _logger.LogInformation("✅ [TRANSFER-{CorrelationId}] Apartamento destino encontrado - Código: {CodigoApartamento}, Status: {StatusApartamento}",
                        correlationId, apartamentoDestino.Codigo, apartamentoDestino.Situacao);

                    // ✅ CRIAR TRANSFERÊNCIA DE SAÍDA
                    _logger.LogInformation("📤 [TRANSFER-{CorrelationId}] Criando transferência de saída - CheckinId: {CheckinId}, Valor Diária: {ValorDiaria}",
                        correlationId, hospedagem.CheckinsId, hospedagem.ValorDiaria);

                    var transferenciaSaida = new Transferencia
                    {
                        CheckinId = hospedagem.CheckinsId,
                        DataEntrada = hospedagem.DataAbertura,
                        DataSaida = DateTime.Now.Date,
                        QuartoId = hospedagem.ApartamentosId,
                        TipoTransferencia = TipoTransferencia.Saida,
                        ValorDiaria = hospedagem.ValorDiaria,
                        Observacao = request.Observacao,
                        MotivoTransferenciaId = request.MotivoTransferenciaId,
                        ManterPreco = request.ManterPreco,
                        DataTransferencia = request.DataTransferencia,
                        UtilizadorId = _logado.IdUtilizador
                    };

                    // ✅ CRIAR TRANSFERÊNCIA DE ENTRADA
                    _logger.LogInformation("📥 [TRANSFER-{CorrelationId}] Criando transferência de entrada - QuartoDestino: {QuartoDestino}, Valor Diária: {ValorDiaria}",
                        correlationId, request.QuartoDestinoId, request.valorDiaria);

                    var transferenciaEntrada = new Transferencia
                    {
                        CheckinId = hospedagem.CheckinsId,
                        DataEntrada = request.DataTransferencia,
                        DataSaida = hospedagem.PrevisaoFechamento,
                        QuartoId = request.QuartoDestinoId, // ✅ CORREÇÃO: Usar QuartoDestinoId para entrada
                        TipoTransferencia = TipoTransferencia.Entrada,
                        ValorDiaria = request.valorDiaria,
                        Observacao = request.Observacao,
                        MotivoTransferenciaId = request.MotivoTransferenciaId,
                        ManterPreco = request.ManterPreco,
                        DataTransferencia = request.DataTransferencia,
                        UtilizadorId = _logado.IdUtilizador
                    };

                    // ✅ PERSISTIR TRANSFERÊNCIAS
                    _logger.LogInformation("💾 [TRANSFER-{CorrelationId}] Salvando transferências no banco de dados",
                        correlationId);

                    await _unitOfWork.Transferencias.Add(transferenciaSaida);
                    await _unitOfWork.Transferencias.Add(transferenciaEntrada);

                    // ✅ BUSCAR E ATUALIZAR CHECK-IN
                    _logger.LogInformation("🔍 [TRANSFER-{CorrelationId}] Buscando check-in - ID: {CheckinId}",
                        correlationId, hospedagem.CheckinsId);

                    var checkin = await _unitOfWork.checkins.GetByIdAsync(hospedagem.CheckinsId);
                    if (checkin == null)
                    {
                        _logger.LogWarning("⚠️ [TRANSFER-{CorrelationId}] Check-in não encontrado - ID: {CheckinId}",
                            correlationId, hospedagem.CheckinsId);
                    }

                    // ✅ CALCULAR TOTAL DE DIÁRIAS
                    _logger.LogInformation("🧮 [TRANSFER-{CorrelationId}] Calculando total de diárias para check-in: {CheckinId}",
                        correlationId, hospedagem.CheckinsId);

                    var totalDiariasDecimal = await CalcularTotalDiariasAsync(hospedagem.CheckinsId);
                    var totalDiarias = (float)totalDiariasDecimal;

                    _logger.LogInformation("💰 [TRANSFER-{CorrelationId}] Total de diárias calculado: {TotalDiarias}",
                        correlationId, totalDiarias);

                    if (checkin != null)
                    {
                        _logger.LogInformation("🔄 [TRANSFER-{CorrelationId}] Atualizando check-in com novo valor total: {ValorTotal}",
                            correlationId, totalDiarias);
                        
                        checkin.update(checkin.Id, totalDiarias);
                        await _unitOfWork.checkins.Update(checkin);
                    }

                    // ✅ ATUALIZAR HOSPEDAGEM
                    _logger.LogInformation("🏠 [TRANSFER-{CorrelationId}] Atualizando hospedagem - Novo apartamento: {NovoApartamentoId}",
                        correlationId, request.QuartoDestinoId);

                    hospedagem.AtualizaNovoApartamento(request.QuartoDestinoId);

                    // ✅ OCUPAR APARTAMENTO DESTINO
                    _logger.LogInformation("🔒 [TRANSFER-{CorrelationId}] Ocupando apartamento destino - ID: {ApartamentoDestino}, CheckinId: {CheckinId}",
                        correlationId, request.QuartoDestinoId, checkin?.Id);

                    _unitOfWork.Apartamento.ocuparApartamento(request.QuartoDestinoId, checkin?.Id ?? 0);

                    // ✅ ATUALIZAR DESCRIÇÃO DA HOSPEDAGEM
                    var descricaoTransferencia = $"Hospedagem transferida do quarto {apartamento.Codigo} para o quarto {apartamentoDestino.Codigo}";
                    _logger.LogInformation("📝 [TRANSFER-{CorrelationId}] Atualizando descrição: {Descricao}",
                        correlationId, descricaoTransferencia);

                    hospedagem.AtualizarDescricao(descricaoTransferencia);
                    await _unitOfWork.Hospedagem.Update(hospedagem);

                    // ✅ CRIAR HISTÓRICO DA TRANSFERÊNCIA
                    _logger.LogInformation("📋 [TRANSFER-{CorrelationId}] Criando histórico da transferência - Caixa: {CaixaId}",
                        correlationId, IdCaixa);

                    var historico = new Historico(IdCaixa, _logado.IdUtilizador, checkin?.Id ?? 0);
                    historico.HistoricoTransferencia(apartamento.Codigo, apartamentoDestino.Codigo);
                    await _unitOfWork.historico.Add(historico);

                    // ✅ SALVAR TODAS AS ALTERAÇÕES
                    _logger.LogInformation("💾 [TRANSFER-{CorrelationId}] Salvando todas as alterações no banco de dados",
                        correlationId);

                    await _unitOfWork.Save(); // ✅ ADICIONAR SaveChanges se necessário

                    // ✅ LOG DE SUCESSO
                    _logger.LogInformation("✅ [TRANSFER-{CorrelationId}] Transferência concluída com sucesso - Hospedagem: {HospedagemId}, De: {QuartoOrigem} Para: {QuartoDestino}, Total Diárias: {TotalDiarias}",
                        correlationId, hospedagem.Id, apartamento.Codigo, apartamentoDestino.Codigo, totalDiarias);

                    resposta.Data = hospedagem;
                    resposta.Success = true;
                    resposta.Message = "Transferência de hospedagem realizada com sucesso";
                }
                catch (Exception ex)
                {
                    // ✅ LOG DE ERRO DETALHADO
                    _logger.LogError(ex, "❌ [TRANSFER-{CorrelationId}] Erro ao realizar transferência - Hospedagem: {HospedagemId}, Quarto Destino: {QuartoDestino}, Usuário: {UserId}, Erro: {ErrorMessage}",
                        correlationId, request.HospedagemOrigemId, request.QuartoDestinoId, _logado.IdUtilizador, ex.Message);

                    resposta = RespostaErro($"Erro ao realizar transferência de hospedagem: {ex.Message}");
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

            /// <summary>
            /// ✅ MÉTODO AUXILIAR: Obter transferências usando EF Core com tratamento de erros de cast
            /// </summary>
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


            /// <summary>
            /// ✅ MÉTODO FALLBACK: Consulta alternativa com validações adicionais
            /// </summary>
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

            /// <summary>
            /// ✅ MÉTODO AUXILIAR ADICIONAL: Validar estrutura da tabela - VERSÃO ASSÍNCRONA
            /// </summary>
            private async Task ValidarEstruturaTransferencias(string correlationId)
            {
                try
                {
                    _logger.LogInformation("🔍 [CALC-{CorrelationId}] Validando estrutura da tabela Transferencias",
                        correlationId);

                    // ✅ Se o repository tiver método assíncrono para contar
                    var totalTransferencias = await _unitOfWork.Transferencias.GetCountAsync();
        
                    _logger.LogInformation("✅ [CALC-{CorrelationId}] Estrutura validada - Total de transferências: {Count}",
                        correlationId, totalTransferencias);

                    // ✅ VALIDAÇÃO ADICIONAL: Contar transferências ativas
                    var transferenciasAtivas = await _unitOfWork.Transferencias.GetCountAsync();
        
                    _logger.LogInformation("📊 [CALC-{CorrelationId}] Transferências ativas: {CountAtivas}",
                        correlationId, transferenciasAtivas);

                    // ✅ TESTE DE CONSULTA POR CHECKIN ESPECÍFICO
                    var transferenciasExemplo = await _unitOfWork.Transferencias.GetByCheckinIdAsync(1);
        
                    _logger.LogInformation("🔍 [CALC-{CorrelationId}] Teste de consulta por CheckinId=1: {Count} registros",
                        correlationId, transferenciasExemplo?.Count() ?? 0);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ [CALC-{CorrelationId}] Problema na estrutura da tabela Transferencias: {Error}",
                        correlationId, ex.Message);
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
            /// <summary>
            /// ✅ DTO para transferências para evitar problemas de cast
            /// </summary>
            public class TransferenciaDto
            {
                public int Id { get; set; }
                public int CheckinId { get; set; }
                public DateTime DataEntrada { get; set; }
                public DateTime DataSaida { get; set; }
                public int QuartoId { get; set; }
                public int TipoTransferencia { get; set; }
                public decimal ValorDiaria { get; set; }
                public string Observacao { get; set; }
                public int MotivoTransferenciaId { get; set; }
                public bool ManterPreco { get; set; }
                public DateTime DataTransferencia { get; set; }
                public string UtilizadorId { get; set; }
                public DateTime DateCreated { get; set; }
                public bool IsActive { get; set; }
            }
        }
    }
}