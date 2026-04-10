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
    public class GetTodosSaldosEmpresasQuery: IRequest<List<EmpresaSaldoDto>>
    {
         public class GetTodosSaldosEmpresasQueryHandler : IRequestHandler<GetTodosSaldosEmpresasQuery, List<EmpresaSaldoDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTodosSaldosEmpresasQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EmpresaSaldoDto>> Handle(GetTodosSaldosEmpresasQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Buscando todos os saldos de empresas");

                var saldos = await _unitOfWork.EmpresaSaldo.GetTodosSaldosAsync();

                var dtos = saldos.Select(s => new EmpresaSaldoDto
                {
                    Id = s.Id,
                    EmpresaId = s.EmpresaId,
                    NomeEmpresa = s.Empresa?.RazaoSocial ?? string.Empty,
                    Saldo = s.Saldo,
                    DataAtualizacao = s.DateCreated,
                    Movimentacoes = s.EmpresaSaldoMovimentos?.Select(m => new EmpresaSaldoMovimentoDto
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
                    }).ToList() ?? new List<EmpresaSaldoMovimentoDto>()
                }).ToList();

                return dtos;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar todos os saldos");
                return new List<EmpresaSaldoDto>();
            }
        }
    }
    }
}