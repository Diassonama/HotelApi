using System;
using System.Collections.Generic;
using System.Linq;
using Hotel.Application.DTOs;
using Hotel.Application.Interfaces;
using Hotel.Domain.Interface;
using MediatR;
using Serilog;

namespace Hotel.Application.EmpresaSaldo.Queries
{
    public class GerarRelatorioAdiantamentosHistoricoQuery : IRequest<byte[]>
    {
        public int? EmpresaId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public class GerarRelatorioAdiantamentosHistoricoQueryHandler : IRequestHandler<GerarRelatorioAdiantamentosHistoricoQuery, byte[]>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IRelatorioSaldoService _relatorioSaldoService;

            public GerarRelatorioAdiantamentosHistoricoQueryHandler(
                IUnitOfWork unitOfWork,
                IRelatorioSaldoService relatorioSaldoService)
            {
                _unitOfWork = unitOfWork;
                _relatorioSaldoService = relatorioSaldoService;
            }

            public async Task<byte[]> Handle(GerarRelatorioAdiantamentosHistoricoQuery request, CancellationToken cancellationToken)
            {
                if (request.DataInicio.HasValue && request.DataFim.HasValue && request.DataFim.Value.Date < request.DataInicio.Value.Date)
                    throw new ArgumentException("A data fim não pode ser menor que a data início.");

                Log.Information("Gerando relatório de adiantamentos/histórico. EmpresaId: {EmpresaId}, DataInicio: {DataInicio}, DataFim: {DataFim}",
                    request.EmpresaId, request.DataInicio, request.DataFim);

                var saldos = new List<EmpresaSaldoDto>();

                if (request.EmpresaId.HasValue)
                {
                    var saldoEmpresa = await _unitOfWork.EmpresaSaldo.GetByEmpresaIdAsync(request.EmpresaId.Value);
                    if (saldoEmpresa != null)
                    {
                        saldos.Add(new EmpresaSaldoDto
                        {
                            Id = saldoEmpresa.Id,
                            EmpresaId = saldoEmpresa.EmpresaId,
                            NomeEmpresa = saldoEmpresa.Empresa?.RazaoSocial ?? string.Empty,
                            Saldo = saldoEmpresa.Saldo,
                            DataAtualizacao = saldoEmpresa.LastModifiedDate != default
                                ? saldoEmpresa.LastModifiedDate
                                : saldoEmpresa.DateCreated
                        });
                    }
                }
                else
                {
                    var todos = await _unitOfWork.EmpresaSaldo.GetTodosSaldosAsync();
                    saldos = todos.Select(s => new EmpresaSaldoDto
                    {
                        Id = s.Id,
                        EmpresaId = s.EmpresaId,
                        NomeEmpresa = s.Empresa?.RazaoSocial ?? string.Empty,
                        Saldo = s.Saldo,
                        DataAtualizacao = s.LastModifiedDate != default
                            ? s.LastModifiedDate
                            : s.DateCreated
                    }).ToList();
                }

                var movimentos = await _unitOfWork.EmpresaSaldo.GetMovimentacoesRelatorioAsync(
                    request.EmpresaId,
                    request.DataInicio,
                    request.DataFim);

                var movimentosDto = movimentos.Select(m => new EmpresaSaldoMovimentoDto
                {
                    Id = m.Id,
                    EmpresaSaldoId = m.EmpresaSaldoId,
                    NomeEmpresa = m.EmpresaSaldo?.Empresa?.RazaoSocial ?? "Sem empresa",
                    Data = m.DateCreated,
                    Valor = m.Valor,
                    TipoLancamento = m.TipoLancamento,
                    Documento = m.Documento,
                    Observacao = m.Observacao,
                    UtilizadorId = m.UtilizadorId,
                    NomeUtilizador = string.Join(" ", new[] { m.Utilizador?.FirstName, m.Utilizador?.LastName }
                        .Where(x => !string.IsNullOrWhiteSpace(x))).Trim()
                        switch
                        {
                            "" => m.Utilizador?.UserName ?? "-",
                            var nome => nome
                        }
                }).ToList();

                var nomeEmpresaFiltro = request.EmpresaId.HasValue
                    ? saldos.FirstOrDefault()?.NomeEmpresa ?? $"Empresa {request.EmpresaId.Value}"
                    : "TODAS AS EMPRESAS";

                return _relatorioSaldoService.GerarRelatorioAdiantamentosHistorico(
                    nomeEmpresaFiltro,
                    saldos,
                    movimentosDto,
                    request.DataInicio,
                    request.DataFim);
            }
        }
    }
}
