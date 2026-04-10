using System;
using System.Collections.Generic;
using Hotel.Application.Interfaces;
using Hotel.Domain.Interface;
using MediatR;
using Serilog;

namespace Hotel.Application.EmpresaSaldo.Queries
{
    public class GerarRelatorioMovimentacoesQuery : IRequest<byte[]>
    {
        public int EmpresaId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public TipoRelatorio TipoRelatorio { get; set; } // Completo, SoCredito, SoDebito

        public class GerarRelatorioMovimentacoesQueryHandler : IRequestHandler<GerarRelatorioMovimentacoesQuery, byte[]>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IRelatorioSaldoService _relatorioService;

            public GerarRelatorioMovimentacoesQueryHandler(
                IUnitOfWork unitOfWork,
                IRelatorioSaldoService relatorioService)
            {
                _unitOfWork = unitOfWork;
                _relatorioService = relatorioService;
            }

            public async Task<byte[]> Handle(GerarRelatorioMovimentacoesQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    Log.Information("Gerando relatório de movimentações para empresa {EmpresaId}", request.EmpresaId);

                    // Buscar empresa
                    var empresa = await _unitOfWork.Empresa.Get(request.EmpresaId);
                    if (empresa == null)
                        throw new Exception("Empresa não encontrada");

                    // Buscar saldo atual
                    var saldo = await _unitOfWork.EmpresaSaldo.GetByEmpresaIdAsync(request.EmpresaId);
                    var saldoAtual = saldo?.Saldo ?? 0;

                    // Buscar movimentações
                    var movimentacoes = request.DataInicio.HasValue && request.DataFim.HasValue
                        ? await _unitOfWork.EmpresaSaldo.GetMovimentacoesEntreDatasAsync(
                            request.EmpresaId,
                            request.DataInicio.Value,
                            request.DataFim.Value)
                        : await _unitOfWork.EmpresaSaldo.GetMovimentacoesAsync(request.EmpresaId);

                    // Converter para DTOs
                    var dtos = movimentacoes.Select(m => new Application.DTOs.EmpresaSaldoMovimentoDto
                    {
                        Id = m.Id,
                        EmpresaSaldoId = m.EmpresaSaldoId,
                        Data = m.DateCreated,
                        Valor = m.Valor,
                        TipoLancamento = m.TipoLancamento,
                        Documento = m.Documento,
                        Observacao = m.Observacao,
                        UtilizadorId = m.UtilizadorId,
                        NomeUtilizador = m.Utilizador?.UserName ?? string.Empty
                    }).ToList();

                    // Gerar PDF conforme tipo solicitado
                    return request.TipoRelatorio switch
                    {
                        TipoRelatorio.SoCredito => _relatorioService.GerarRelatorioCreditos(
                            empresa.RazaoSocial,
                            dtos,
                            request.DataInicio,
                            request.DataFim),

                        TipoRelatorio.SoDebito => _relatorioService.GerarRelatorioDebitos(
                            empresa.RazaoSocial,
                            dtos,
                            request.DataInicio,
                            request.DataFim),

                        _ => _relatorioService.GerarRelatorioMovimentacoes(
                            empresa.RazaoSocial,
                            saldoAtual,
                            dtos,
                            request.DataInicio,
                            request.DataFim)
                    };
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Erro ao gerar relatório de movimentações");
                    throw;
                }
            }
        }
    }

    public enum TipoRelatorio
    {
        Completo = 0,
        SoCredito = 1,
        SoDebito = 2
    }
}