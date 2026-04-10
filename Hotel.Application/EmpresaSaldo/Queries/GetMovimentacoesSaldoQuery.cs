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
    public class GetMovimentacoesSaldoQuery: IRequest<List<EmpresaSaldoMovimentoDto>>
    {
        public int EmpresaId { get; set; }


        public class GetMovimentacoesSaldoQueryHandler : IRequestHandler<GetMovimentacoesSaldoQuery, List<EmpresaSaldoMovimentoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMovimentacoesSaldoQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EmpresaSaldoMovimentoDto>> Handle(GetMovimentacoesSaldoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Buscando movimentações da empresa {EmpresaId}", request.EmpresaId);

                var movimentacoes = await _unitOfWork.EmpresaSaldo.GetMovimentacoesAsync(request.EmpresaId);

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
                    NomeUtilizador = m.Utilizador?.UserName ?? string.Empty
                }).ToList();

                return dtos;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar movimentações da empresa {EmpresaId}", request.EmpresaId);
                return new List<EmpresaSaldoMovimentoDto>();
            }
        }
    }
    }
}