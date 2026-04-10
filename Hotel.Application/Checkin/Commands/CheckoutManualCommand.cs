// Hotel.Application/Checkin/Commands/CheckoutManualCommand.cs - VERSÃO FINAL
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Checkin.Commands
{
    public class CheckoutManualCommand : IRequest<BaseCommandResponse>
    {
        public int CheckinId { get; set; }
        public DateTime Data { get; set; }

        public class Handler : IRequestHandler<CheckoutManualCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IRackNotificationService _rackNotificationService;
            private readonly ILogger<Handler> _logger;

            public Handler(IUnitOfWork unitOfWork, IRackNotificationService rackNotificationService, ILogger<Handler> logger)
            {
                _unitOfWork = unitOfWork;
                _rackNotificationService = rackNotificationService;
                _logger = logger;
            }

            public async Task<BaseCommandResponse> Handle(CheckoutManualCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();
                var correlationId = Guid.NewGuid().ToString("N")[..8];

                try
                {
                    _logger.LogInformation("🏁 [CHECKOUT-MANUAL-{CorrelationId}] Executando checkout manual - CheckinId: {CheckinId}, Data: {Data}",
                        correlationId, request.CheckinId, request.Data);

                    // Executar stored procedure usando o repositório
                    var checkInIdParam = new SqlParameter("@CheckinId", request.CheckinId);
                    var dataParam = new SqlParameter("@Data", request.Data.Date);

                    await _unitOfWork.checkins.ExecuteSqlRawAsync(
                        "EXEC [dbo].[CheckoutManual] @CheckinId, @Data",
                        checkInIdParam,
                        dataParam
                    );

                    // Atualiza todas as telas de rack conectadas
                    await _rackNotificationService.NotifyRackUpdateAsync();

                    _logger.LogInformation("✅ [CHECKOUT-MANUAL-{CorrelationId}] Stored procedure executada com sucesso",
                        correlationId);

                    response.Success = true;
                    response.Message = "Checkout manual realizado com sucesso";
                    response.Data = new
                    {
                        CheckinId = request.CheckinId,
                        DataSaida = request.Data
                    };

                    _logger.LogInformation("✅ [CHECKOUT-MANUAL-{CorrelationId}] Checkout concluído - CheckinId: {CheckinId}",
                        correlationId, request.CheckinId);

                    return response;
                }
                catch (SqlException sqlEx)
                {
                    _logger.LogError(sqlEx, "💥 [CHECKOUT-MANUAL-{CorrelationId}] Erro SQL - CheckinId: {CheckinId}, Número: {Number}, Mensagem: {Message}",
                        correlationId, request.CheckinId, sqlEx.Number, sqlEx.Message);

                    response.Success = false;
                    response.Message = "Erro ao processar checkout manual";
                    response.Errors = new List<string> 
                    { 
                        sqlEx.Message,
                        $"Código SQL: {sqlEx.Number}"
                    };
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "💥 [CHECKOUT-MANUAL-{CorrelationId}] Erro ao processar checkout manual - CheckinId: {CheckinId}",
                        correlationId, request.CheckinId);

                    response.Success = false;
                    response.Message = "Erro ao processar checkout manual";
                    response.Errors = new List<string> { ex.Message };
                    return response;
                }
            }
        }
    }
}