using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hotel.Application.Pagamento.Queries.GetPagamentoById
{
    public class GetPagamentoByIdQueryHandler : IRequestHandler<GetPagamentoByIdQuery, BaseQueryResponse<Domain.Entities.Pagamento>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetPagamentoByIdQueryHandler> _logger;

        public GetPagamentoByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPagamentoByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseQueryResponse<Domain.Entities.Pagamento>> Handle(GetPagamentoByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseQueryResponse<Domain.Entities.Pagamento>();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("🔍 [GET-PAGAMENTO-BY-ID-{CorrelationId}] Buscando pagamento por ID: {Id}",
                    correlationId, request.Id);

                // ✅ VALIDAÇÃO DO ID
                if (request.Id <= 0)
                {
                    _logger.LogWarning("⚠️ [GET-PAGAMENTO-BY-ID-{CorrelationId}] ID inválido fornecido: {Id}",
                        correlationId, request.Id);

                    response.Success = false;
                    response.Message = "ID do pagamento deve ser maior que zero";
                    response.Errors = new List<string> { "ID inválido" };
                    return response;
                }

                // ✅ BUSCAR PAGAMENTO COM RELACIONAMENTOS
                var pagamento = await _unitOfWork.pagamentos.GetByIdAsync(request.Id);

                if (pagamento == null)
                {
                    _logger.LogWarning("⚠️ [GET-PAGAMENTO-BY-ID-{CorrelationId}] Pagamento não encontrado - ID: {Id}",
                        correlationId, request.Id);

                    response.Success = false;
                    response.Message = $"Pagamento com ID {request.Id} não encontrado";
                    response.Data = null;
                    return response;
                }

                _logger.LogInformation("✅ [GET-PAGAMENTO-BY-ID-{CorrelationId}] Pagamento encontrado - ID: {Id}, Valor: {Valor}, Status: {Status}, CheckinId: {CheckinId}",
                    correlationId, pagamento.Id, pagamento.Valor, pagamento.Status, pagamento.OrigemId);

                response.Success = true;
                response.Message = "Pagamento encontrado com sucesso";
                response.Data = pagamento;
                response.Count = 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GET-PAGAMENTO-BY-ID-{CorrelationId}] Erro ao buscar pagamento por ID {Id}: {Message}",
                    correlationId, request.Id, ex.Message);

                response.Success = false;
                response.Message = $"Erro ao buscar pagamento: {ex.Message}";
                response.Data = null;
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}