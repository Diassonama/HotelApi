using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.DTOs;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Serilog;

namespace Hotel.Application.EmpresaSaldo.Queries
{
    public class GetSaldoEmpresaQuery: IRequest<BaseCommandResponse>
    {
        public int EmpresaId { get; set; }

 public class GetSaldoEmpresaQueryHandler : IRequestHandler<GetSaldoEmpresaQuery, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSaldoEmpresaQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseCommandResponse> Handle(GetSaldoEmpresaQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Buscando saldo da empresa {EmpresaId}", request.EmpresaId);

                var empresaSaldo = await _unitOfWork.EmpresaSaldo.GetByEmpresaIdAsync(request.EmpresaId);

                if (empresaSaldo == null)
                    return null;

                var dto = new EmpresaSaldoDto
                {
                    Id = empresaSaldo.Id,
                    EmpresaId = empresaSaldo.EmpresaId,
                    NomeEmpresa = empresaSaldo.Empresa?.RazaoSocial ?? string.Empty,
                    Saldo = empresaSaldo.Saldo,
                    DataAtualizacao = empresaSaldo.DateCreated,
                    Movimentacoes = empresaSaldo.EmpresaSaldoMovimentos?.Select(m => new EmpresaSaldoMovimentoDto
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
                };

                var resposta = new BaseCommandResponse();


                if(empresaSaldo==null){
                    resposta.Message="Empresa saldo não encontrada";
                    resposta.Success= false;
                    return resposta;
                }

                resposta.Message ="Saldo da empresa carregado com sucesso";
                resposta.Data = dto;
                resposta.Success = true;

                return resposta;


            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar saldo da empresa {EmpresaId}", request.EmpresaId);
                return null;
            }
        }
    }



    }
}