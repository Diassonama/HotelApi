using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.DTOs;
using Hotel.Domain.Interface;
using MediatR;
using Serilog;

namespace Hotel.Application.EmpresaSaldo.Queries
{
    public class GetMovimentacoesPeriodoQuery: IRequest<List<EmpresaSaldoMovimentoDto>>
    {
        public int EmpresaId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

 public class GetMovimentacoesPeriodoQueryHandler : IRequestHandler<GetMovimentacoesPeriodoQuery, List<EmpresaSaldoMovimentoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMovimentacoesPeriodoQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EmpresaSaldoMovimentoDto>> Handle(GetMovimentacoesPeriodoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Buscando movimentações da empresa {EmpresaId} de {DataInicio} a {DataFim}", 
                    request.EmpresaId, request.DataInicio, request.DataFim);

                var movimentacoes = await _unitOfWork.EmpresaSaldo.GetMovimentacoesEntreDatasAsync(
                    request.EmpresaId, 
                    request.DataInicio, 
                    request.DataFim);

                     Log.Information("Total de movimentações encontradas: {Count}", movimentacoes.Count);
        
        // Debug: verifica se os dados estão carregados
        foreach (var mov in movimentacoes.Take(1))
        {
            Log.Information("Debug - EmpresaSaldo: {EmpresaSaldo}, Empresa: {Empresa}, Utilizador: {Utilizador}",
                mov.EmpresaSaldo != null ? "OK" : "NULL",
                mov.EmpresaSaldo?.Empresa != null ? "OK" : "NULL",
                mov.Utilizador != null ? "OK" : "NULL");
        }

                var dtos = movimentacoes.Select(m => new EmpresaSaldoMovimentoDto
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
                    NomeUtilizador = m.Utilizador?.UserName ?? "Utilizador desconhecido"
                }).ToList();

                return dtos;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar movimentações do período");
                return new List<EmpresaSaldoMovimentoDto>();
            }
        }
    }



    }
}