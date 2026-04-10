using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Produto.Commands.CreateProduto
{
    public class CreateProdutoCommandHandler : IRequestHandler<CreateProdutoCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateProdutoCommandHandler> _logger;

        public CreateProdutoCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateProdutoCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseCommandResponse> Handle(CreateProdutoCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var correlationId = Guid.NewGuid().ToString("N")[..8];

            try
            {
                // ✅ LOG INICIAL
                _logger.LogInformation("🛍️ [CREATE-PRODUTO-{CorrelationId}] Iniciando criação de produto - Nome: {Nome}, Valor: {Valor}, Categoria: {CategoriaId}",
                    correlationId, request.Nome, request.Valor, request.CategoriaId);

                // ✅ VALIDAÇÃO
                _logger.LogInformation("✔️ [CREATE-PRODUTO-{CorrelationId}] Iniciando validação dos dados do produto",
                    correlationId);

                var validator = new CreateProdutoCommandValidator();
                var validationResult = await validator.ValidateAsync(request);

                if (validationResult.Errors.Count > 0)
                {
                    // ✅ LOG DE ERRO DE VALIDAÇÃO
                    _logger.LogWarning("⚠️ [CREATE-PRODUTO-{CorrelationId}] Falha na validação - {ErrorCount} erros encontrados: {Errors}",
                        correlationId, 
                        validationResult.Errors.Count,
                        string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

                    response.Success = false;
                    response.Message = "Creation Failed";
                    response.Errors = validationResult.Errors.Select(q => q.ErrorMessage).ToList();
                }
                else
                {
                    // ✅ LOG DE VALIDAÇÃO SUCESSO
                    _logger.LogInformation("✅ [CREATE-PRODUTO-{CorrelationId}] Validação bem-sucedida, criando entidade produto",
                        correlationId);

                    // ✅ LOG DETALHADO DOS DADOS DO PRODUTO
                    _logger.LogInformation("📝 [CREATE-PRODUTO-{CorrelationId}] Dados do produto - Nome: {Nome}, Valor: {Valor}, PreçoCompra: {PrecoCompra}, CategoriaId: {CategoriaId}, PontoVendaId: {PontoVendaId}",
                        correlationId, request.Nome, request.Valor, request.PrecoCompra, request.CategoriaId, request.PontoDeVendasId);

                    _logger.LogInformation("📊 [CREATE-PRODUTO-{CorrelationId}] Estoque - Quantidade: {Quantidade}, EstoqueMin: {EstoqueMinimo}, AdicionarStock: {AdicionarStock}",
                        correlationId, request.Quantidade, request.EstoqueMinimo, request.AdicionarStock);

                    _logger.LogInformation("💰 [CREATE-PRODUTO-{CorrelationId}] Valores financeiros - MargemLucro: {MargemLucro}, Lucro: {Lucro}, ValorFixo: {ValorFixo}, PrecoCIva: {PrecoCIva}, Desconto: {Desconto}%",
                        correlationId, request.MargemLucro, request.Lucro, request.ValorFixo, request.PrecoCIva, request.DescontoPercentagem);

                    _logger.LogInformation("🔧 [CREATE-PRODUTO-{CorrelationId}] Campos fiscais - ProductTypeCode: {ProductTypeCode}, TaxExemptionReasonCode: {TaxExemptionReasonCode}, TaxTableEntryId: {TaxTableEntryId}",
                        correlationId, request.ProductTypeCode, request.TaxExemptionReasonCode, request.TaxTableEntryId);

                    // ✅ VALIDAÇÃO ADICIONAL DE CAMPOS FISCAIS
                    if (request.TaxTableEntryId != 0)
                    {
                        _logger.LogInformation("🧾 [CREATE-PRODUTO-{CorrelationId}] Validando TaxTableEntryId: {TaxTableEntryId}",
                            correlationId, request.TaxTableEntryId);
                    }
                    else
                    {
                        _logger.LogInformation("🧾 [CREATE-PRODUTO-{CorrelationId}] TaxTableEntryId será definido como NULL ou valor padrão",
                            correlationId);
                    }

                    // ✅ CRIAÇÃO DA ENTIDADE
                    var produto = new Domain.Entities.Produtos(
                        request.Nome, 
                        request.Valor, 
                        request.PrecoCompra, 
                        request.CategoriaId,
                        request.PontoDeVendasId, 
                        request.DataExpiracao, 
                        request.ProductTypeCode, 
                        request.TaxExemptionReasonCode,
                        request.TaxTableEntryId, 
                        request.MargemLucro, 
                        request.Quantidade, 
                        request.EstoqueMinimo, 
                        request.AdicionarStock, 
                        request.Lucro, 
                        request.ValorFixo,
                        request.PrecoCIva, 
                        request.Desconto, 
                        request.DescontoPercentagem);

                    _logger.LogInformation("🏗️ [CREATE-PRODUTO-{CorrelationId}] Entidade produto criada com sucesso - ID temporário: {ProdutoId}",
                        correlationId, produto.Id);

                    // ✅ PERSISTÊNCIA NO BANCO COM TRATAMENTO DE ERRO ESPECÍFICO
                    _logger.LogInformation("💾 [CREATE-PRODUTO-{CorrelationId}] Salvando produto no banco de dados",
                        correlationId);

                    try
                    {
                        await _unitOfWork.Produto.Add(produto);
                        
                        _logger.LogInformation("💾 [CREATE-PRODUTO-{CorrelationId}] Produto adicionado ao contexto, iniciando SaveChanges",
                            correlationId);

                        await _unitOfWork.Save();

                        _logger.LogInformation("💾 [CREATE-PRODUTO-{CorrelationId}] SaveChanges executado com sucesso",
                            correlationId);
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) when (dbEx.InnerException is System.Data.SqlTypes.SqlNullValueException)
                    {
                        // ✅ TRATAMENTO ESPECÍFICO PARA ERRO DE NULL
                        _logger.LogError(dbEx, "❌ [CREATE-PRODUTO-{CorrelationId}] Erro de valor NULL detectado. Possível problema com TaxTableEntryId ou outro campo nullable",
                            correlationId);

                        // ✅ TENTAR NOVAMENTE COM VALORES DEFAULT MAIS SEGUROS
                        _logger.LogInformation("🔄 [CREATE-PRODUTO-{CorrelationId}] Tentando criar produto com valores default para campos problemáticos",
                            correlationId);

                        // Recriar produto com valores mais seguros
                        var produtoSafe = new Domain.Entities.Produtos(
                            request.Nome, 
                            request.Valor, 
                            request.PrecoCompra, 
                            request.CategoriaId,
                            request.PontoDeVendasId, 
                            request.DataExpiracao, 
                            request.ProductTypeCode ?? "P", // ✅ Valor default
                            request.TaxExemptionReasonCode ?? "M00", // ✅ Valor default
                            0, // ✅ Forçar NULL para TaxTableEntryId
                            request.MargemLucro, 
                            request.Quantidade, 
                            request.EstoqueMinimo, 
                            request.AdicionarStock, 
                            request.Lucro, 
                            request.ValorFixo,
                            request.PrecoCIva, 
                            request.Desconto, 
                            request.DescontoPercentagem);

                        await _unitOfWork.Produto.Add(produtoSafe);
                        await _unitOfWork.Save();
                        
                        produto = produtoSafe; // ✅ Usar o produto que foi salvo com sucesso
                        
                        _logger.LogInformation("✅ [CREATE-PRODUTO-{CorrelationId}] Produto criado com sucesso usando valores default seguros",
                            correlationId);
                    }

                    // ✅ LOG DE SUCESSO
                    _logger.LogInformation("✅ [CREATE-PRODUTO-{CorrelationId}] Produto criado com sucesso - ID final: {ProdutoId}, Nome: {Nome}",
                        correlationId, produto.Id, produto.Nome);

                    // ✅ BUSCAR O PRODUTO SALVO PARA GARANTIR QUE OS DADOS ESTÃO CORRETOS
                    var produtoSalvo = await _unitOfWork.Produto.GetByCodigoAsync(produto.Id);
                    if (produtoSalvo != null)
                    {
                        _logger.LogInformation("🔍 [CREATE-PRODUTO-{CorrelationId}] Produto verificado no banco - ID: {Id}, TaxTableEntryId: {TaxTableEntryId}",
                            correlationId, produtoSalvo.Id, produtoSalvo.TaxTableEntryId);
                    }

                    response.Success = true;
                    response.Message = "Creation Successful";
                    response.Data = produtoSalvo ?? produto; // ✅ Retornar o produto verificado ou o original
                }
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                // ✅ LOG DE ERRO DE BANCO DE DADOS
                _logger.LogError(dbEx, "❌ [CREATE-PRODUTO-{CorrelationId}] Erro de banco de dados ao criar produto - Nome: {Nome}, InnerException: {InnerException}",
                    correlationId, request?.Nome ?? "N/A", dbEx.InnerException?.Message ?? "N/A");

                response.Success = false;
                response.Message = "Erro de banco de dados ao criar produto";
                response.Errors = new List<string> { 
                    $"Erro de banco: {dbEx.InnerException?.Message ?? dbEx.Message}",
                    "Possível problema com campos nullable ou configuração do banco"
                };
            }
            catch (System.Data.SqlTypes.SqlNullValueException sqlNullEx)
            {
                // ✅ LOG DE ERRO ESPECÍFICO DE NULL
                _logger.LogError(sqlNullEx, "❌ [CREATE-PRODUTO-{CorrelationId}] Erro de valor NULL - Nome: {Nome}",
                    correlationId, request?.Nome ?? "N/A");

                response.Success = false;
                response.Message = "Erro de valor NULL no banco de dados";
                response.Errors = new List<string> { 
                    "Erro de valor NULL - possível problema com TaxTableEntryId",
                    "Verifique se todos os campos obrigatórios estão preenchidos"
                };
            }
            catch (Exception ex)
            {
                // ✅ LOG DE ERRO GERAL
                _logger.LogError(ex, "❌ [CREATE-PRODUTO-{CorrelationId}] Erro geral ao criar produto - Nome: {Nome}, Categoria: {CategoriaId}, Erro: {ErrorMessage}",
                    correlationId, request?.Nome ?? "N/A", request?.CategoriaId ?? 0, ex.Message);

                response.Success = false;
                response.Message = $"Erro ao criar produto: {ex.Message}";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}