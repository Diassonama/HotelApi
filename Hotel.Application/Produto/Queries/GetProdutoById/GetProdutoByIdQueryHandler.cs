using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hotel.Application.Produto.Queries.GetProdutoById
{
    public class GetProdutoByIdQueryHandler : IRequestHandler<GetProdutoByIdQuery, BaseQueryResponse<Domain.Entities.Produtos>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetProdutoByIdQueryHandler> _logger;

        public GetProdutoByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetProdutoByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseQueryResponse<Domain.Entities.Produtos>> Handle(GetProdutoByIdQuery request, CancellationToken cancellationId)
        {
            var response = new BaseQueryResponse<Domain.Entities.Produtos>();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("🔍 [GET-PRODUTO-BY-ID-{CorrelationId}] Buscando produto por ID: {Id}",
                    correlationId, request.Id);

                // ✅ VALIDAÇÃO DO ID
                if (request.Id <= 0)
                {
                    _logger.LogWarning("⚠️ [GET-PRODUTO-BY-ID-{CorrelationId}] ID inválido fornecido: {Id}",
                        correlationId, request.Id);

                    response.Success = false;
                    response.Message = "ID do produto deve ser maior que zero";
                    response.Errors = new List<string> { "ID inválido" };
                    return response;
                }

                // ✅ BUSCAR PRODUTO COM RELACIONAMENTOS
                var produto = await _unitOfWork.Produto.GetByCodigoAsync(request.Id);

                if (produto == null)
                {
                    _logger.LogWarning("⚠️ [GET-PRODUTO-BY-ID-{CorrelationId}] Produto não encontrado - ID: {Id}",
                        correlationId, request.Id);

                    response.Success = false;
                    response.Message = $"Produto com ID {request.Id} não encontrado";
                    response.Data = null;
                    return response;
                }

                _logger.LogInformation("✅ [GET-PRODUTO-BY-ID-{CorrelationId}] Produto encontrado - ID: {Id}, Nome: {Nome}, TaxTableEntryId: {TaxTableEntryId}",
                    correlationId, produto.Id, produto.Nome, produto.TaxTableEntryId);

                response.Success = true;
                response.Message = "Produto encontrado com sucesso";
                response.Data = produto;
                response.Count = 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GET-PRODUTO-BY-ID-{CorrelationId}] Erro ao buscar produto por ID {Id}: {Message}",
                    correlationId, request.Id, ex.Message);

                response.Success = false;
                response.Message = $"Erro ao buscar produto: {ex.Message}";
                response.Data = null;
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}