using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hotel.Application.Produto.Queries.GetAllProdutos
{
    public class GetAllProdutosQueryHandler : IRequestHandler<GetAllProdutosQuery, BaseQueryResponse<List<Domain.Entities.Produtos>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllProdutosQueryHandler> _logger;

        public GetAllProdutosQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllProdutosQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseQueryResponse<List<Domain.Entities.Produtos>>> Handle(GetAllProdutosQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseQueryResponse<List<Domain.Entities.Produtos>>();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                _logger.LogInformation("🔍 [GET-ALL-PRODUTOS-{CorrelationId}] Iniciando busca de produtos - Filtros: CategoriaId={CategoriaId}, PontoVendasId={PontoVendasId}, ApenasAtivos={ApenasAtivos}",
                    correlationId, request.CategoriaId, request.PontoDeVendasId, request.ApenasAtivos);

                // ✅ BUSCAR PRODUTOS COM RELACIONAMENTOS
                var produtos = await _unitOfWork.Produto.GetAllAsync();

                // ✅ CONVERTER PARA LISTA E APLICAR FILTROS
                var produtosList = produtos?.ToList() ?? new List<Domain.Entities.Produtos>();

                if (produtosList.Any())
                {
                    _logger.LogInformation("🔍 [GET-ALL-PRODUTOS-{CorrelationId}] {Count} produtos encontrados antes dos filtros",
                        correlationId, produtosList.Count);

                    // ✅ APLICAR FILTROS UM POR UM
                    if (request.CategoriaId.HasValue)
                    {
                        produtosList = produtosList.Where(p => p.CategoriaId == request.CategoriaId.Value).ToList();
                        _logger.LogInformation("🔍 [GET-ALL-PRODUTOS-{CorrelationId}] Após filtro CategoriaId {CategoriaId}: {Count} produtos",
                            correlationId, request.CategoriaId.Value, produtosList.Count);
                    }

                    if (request.PontoDeVendasId.HasValue)
                    {
                        produtosList = produtosList.Where(p => p.PontoDeVendasId == request.PontoDeVendasId.Value).ToList();
                        _logger.LogInformation("🔍 [GET-ALL-PRODUTOS-{CorrelationId}] Após filtro PontoDeVendasId {PontoVendasId}: {Count} produtos",
                            correlationId, request.PontoDeVendasId.Value, produtosList.Count);
                    }

                    if (request.ApenasAtivos.HasValue && request.ApenasAtivos.Value)
                    {
                        produtosList = produtosList.ToList();
                        _logger.LogInformation("🔍 [GET-ALL-PRODUTOS-{CorrelationId}] Após filtro apenas ativos: {Count} produtos",
                            correlationId, produtosList.Count);
                    }

                    if (request.ApenasDisponiveis.HasValue && request.ApenasDisponiveis.Value)
                    {
                        produtosList = produtosList.Where(p => p.EstaDisponivel).ToList();
                        _logger.LogInformation("🔍 [GET-ALL-PRODUTOS-{CorrelationId}] Após filtro apenas disponíveis: {Count} produtos",
                            correlationId, produtosList.Count);
                    }

                    if (!string.IsNullOrWhiteSpace(request.Nome))
                    {
                        produtosList = produtosList.Where(p => 
                            !string.IsNullOrEmpty(p.Nome) && 
                            p.Nome.ToLower().Contains(request.Nome.ToLower())).ToList();
                        _logger.LogInformation("🔍 [GET-ALL-PRODUTOS-{CorrelationId}] Após filtro nome '{Nome}': {Count} produtos",
                            correlationId, request.Nome, produtosList.Count);
                    }
                }

                _logger.LogInformation("✅ [GET-ALL-PRODUTOS-{CorrelationId}] Busca concluída - {Count} produtos encontrados",
                    correlationId, produtosList.Count);

                response.Success = true;
                response.Message = "Produtos recuperados com sucesso";
                response.Data = produtosList;
                response.Count = produtosList.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ [GET-ALL-PRODUTOS-{CorrelationId}] Erro ao buscar produtos: {Message}",
                    correlationId, ex.Message);

                response.Success = false;
                response.Message = $"Erro ao buscar produtos: {ex.Message}";
                response.Data = new List<Domain.Entities.Produtos>();
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}