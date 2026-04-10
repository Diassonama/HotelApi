using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hotel.Application.Pagamento.Queries.GetPagamentosByCheckinId
{
    public class GetPagamentosByCheckinIdQueryHandler : IRequestHandler<GetPagamentosByCheckinIdQuery, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetPagamentosByCheckinIdQueryHandler> _logger;

        public GetPagamentosByCheckinIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPagamentosByCheckinIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseCommandResponse> Handle(GetPagamentosByCheckinIdQuery request, CancellationToken cancellationToken)
        {
           // var response = new BaseQueryResponse<List<Domain.Entities.Pagamento>>();
              var response= new BaseCommandResponse();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("🔍 [GET-PAGAMENTOS-BY-CHECKIN-{CorrelationId}] Buscando pagamentos por CheckinId: {CheckinId}",
                    correlationId, request.CheckinId);

                // ✅ VALIDAÇÃO DO CHECKIN ID
                if (request.CheckinId <= 0)
                {
                    _logger.LogWarning("⚠️ [GET-PAGAMENTOS-BY-CHECKIN-{CorrelationId}] CheckinId inválido fornecido: {CheckinId}",
                        correlationId, request.CheckinId);

                    response.Success = false;
                    response.Message = "ID do check-in deve ser maior que zero";
                    response.Errors = new List<string> { "CheckinId inválido" };
                    response.Data = new List<Domain.Entities.Pagamento>();
                    return response;
                }

                // ✅ VERIFICAR SE O CHECK-IN EXISTE
                var checkinExiste = await _unitOfWork.checkins.GetByIdAsync(request.CheckinId);
                if (checkinExiste == null)
                {
                    _logger.LogWarning("⚠️ [GET-PAGAMENTOS-BY-CHECKIN-{CorrelationId}] Check-in não encontrado - ID: {CheckinId}",
                        correlationId, request.CheckinId);

                    response.Success = false;
                    response.Message = $"Check-in com ID {request.CheckinId} não encontrado";
                    response.Data = new List<Domain.Entities.Pagamento>();
                    return response;
                }

                _logger.LogInformation("🔍 [GET-PAGAMENTOS-BY-CHECKIN-{CorrelationId}] Check-in encontrado - ID: {CheckinId}, Status: {Status}",
                    correlationId, checkinExiste.Id, checkinExiste.situacaoDoPagamento);

                // ✅ BUSCAR PAGAMENTOS COM RELACIONAMENTOS
                var pagamentos = await _unitOfWork.pagamentos. GetValorTotalByCheckinIdAsync(request.CheckinId);

                // Certifique-se de que 'pagamentos' é uma coleção (ex: List<Pagamento>)
                var pagamentosList = pagamentos as IEnumerable<Domain.Entities.Pagamento> ?? new List<Domain.Entities.Pagamento>();

                if (pagamentosList == null || !pagamentosList.Any())
                {
                    _logger.LogWarning("⚠️ [GET-PAGAMENTOS-BY-CHECKIN-{CorrelationId}] Nenhum pagamento encontrado para CheckinId: {CheckinId}",
                        correlationId, request.CheckinId);

                    response.Success = true;
                    response.Message = $"Nenhum pagamento encontrado para o check-in {request.CheckinId}";
                    response.Data = new List<Domain.Entities.Pagamento>();
                   
                _logger.LogInformation("✅ [GET-PAGAMENTOS-BY-CHECKIN-{CorrelationId}] Pagamentos encontrados - CheckinId: {CheckinId}, Count: {Count}, ValorTotal: {ValorTotal}",
                    correlationId, request.CheckinId, pagamentosList.Count(), pagamentosList.Sum(p => p.Valor));

                // ✅ ORDENAR PAGAMENTOS POR DATA MAIS RECENTE
                var pagamentosOrdenados = pagamentosList
                    .OrderByDescending(p => p.DateCreated)
                    .ToList();

                response.Success = true;
                response.Message = $"Pagamentos encontrados com sucesso para o check-in {request.CheckinId}";
                response.Data = pagamentos;
                
               /*  response.Message = $"Pagamentos encontrados com sucesso para o check-in {request.CheckinId}";
                response.Data = pagamentosOrdenados;
                response.Count = pagamentosOrdenados.Count; */
            }
        }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GET-PAGAMENTOS-BY-CHECKIN-{CorrelationId}] Erro ao buscar pagamentos por CheckinId {CheckinId}: {Message}",
                    correlationId, request.CheckinId, ex.Message);

                response.Success = false;
                response.Message = $"Erro ao buscar pagamentos: {ex.Message}";
                response.Data = new List<Domain.Entities.Pagamento>();
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}