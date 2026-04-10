using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hotel.Application.DTOs;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Contas.Queries
{
    public class GerarRelatorioContasReceberQuery : IRequest<BaseCommandResponse>
    {
        public int? EmpresaId { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }

        public class Handler : IRequestHandler<GerarRelatorioContasReceberQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _uow;
            private readonly IRelatorioContasService _relatorioService;

            public Handler(IUnitOfWork uow, IRelatorioContasService relatorioService)
            {
                _uow = uow;
                _relatorioService = relatorioService;
            }

            public async Task<BaseCommandResponse> Handle(GerarRelatorioContasReceberQuery request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();

                try
                {
                    if (request.DataInicio.HasValue && request.DataFim.HasValue && request.DataFim.Value.Date < request.DataInicio.Value.Date)
                        throw new ArgumentException("A data fim não pode ser menor que a data início.");

                    var contas = await _uow.ContasReceber.GetRelatorioAsync(
                        request.EmpresaId,
                        request.DataInicio,
                        request.DataFim);

                    var dtos = contas.Select(c => new ContaReceberDto
                    {
                        Id = c.Id,
                        EmpresaId = c.EmpresaId,
                        NomeEmpresa = c.Empresa?.RazaoSocial,
                        CheckinId = c.CheckinsId,
                        ValorTotal = c.ValorTotal,
                        ValorPago = c.ValorPago,
                        Saldo = c.Saldo,
                        DataEmissao = c.DataEmissao,
                        DataVencimento = c.DataVencimento,
                        Documento = c.Documento,
                        Observacao = c.Observacao,
                        Estado = c.Estado
                    }).ToList();

                    var filtroEmpresa = request.EmpresaId.HasValue
                        ? (contas.FirstOrDefault()?.Empresa?.RazaoSocial ?? $"Empresa {request.EmpresaId.Value}")
                        : "TODAS AS EMPRESAS";

                    var pdf = _relatorioService.GerarRelatorioContasReceber(
                        dtos,
                        request.DataInicio,
                        request.DataFim,
                        filtroEmpresa);

                    response.Success = true;
                    response.Message = "Relatório gerado com sucesso";
                    response.Data = pdf;

                    return response;
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = ex.Message;
                    return response;
                }
            }
        }
    }
}