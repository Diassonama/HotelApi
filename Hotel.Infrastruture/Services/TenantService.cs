using System;
using System.Data.SqlClient;
using Hotel.Application.Interfaces;
using Hotel.Domain.Tenant.Entities;
using Hotel.Infrastruture.Persistence.Context;
//using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hotel.Infrastruture.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private Tenant _currentTenant;
        private readonly IConfiguration _configuration;
        private readonly TenantContext _tenantContext;
        private readonly ILogger<TenantService> _logger;

        public TenantService(
            ITenantRepository tenantRepository, 
            IConfiguration configuration, 
            TenantContext tenantContext, 
            Tenant currentTenant,
            ILogger<TenantService> logger)
        {
            _tenantRepository = tenantRepository;
            _configuration = configuration;
            _tenantContext = tenantContext;
            _currentTenant = currentTenant;
            _logger = logger;
            
            _logger.LogInformation("=== TenantService inicializado ===");
            _logger.LogDebug("TenantRepository: {TenantRepository}", _tenantRepository?.GetType().Name ?? "NULL");
            _logger.LogDebug("TenantContext: {TenantContext}", _tenantContext?.GetType().Name ?? "NULL");
            _logger.LogDebug("CurrentTenant: {CurrentTenant}", _currentTenant?.Id ?? "NULL");
        }

        public async Task<Tenant> ResolveTenantAsync(string tenantId)
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            try
            {
                _logger.LogInformation("=== ResolveTenantAsync [{RequestId}] ===", requestId);
                _logger.LogInformation("Buscando tenant: '{TenantId}' [{RequestId}]", tenantId, requestId);

                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    _logger.LogWarning("TenantId é nulo ou vazio [{RequestId}]", requestId);
                    return null;
                }

                if (_tenantRepository == null)
                {
                    _logger.LogError("ITenantRepository é NULL [{RequestId}]", requestId);
                    throw new InvalidOperationException("TenantRepository não foi injetado");
                }

                _logger.LogDebug("Chamando _tenantRepository.GetTenantByIdAsync('{TenantId}') [{RequestId}]", tenantId, requestId);
                
                var tenant = await _tenantRepository.GetTenantByIdAsync(tenantId);
                
                if (tenant != null)
                {
                    _logger.LogInformation("✅ Tenant encontrado: ID='{TenantId}', Name='{TenantName}', Database='{DatabaseName}' [{RequestId}]", 
                        tenant.Id, tenant.Name, tenant.DatabaseName, requestId);
                    _logger.LogDebug("Tenant details: ConnectionString='{ConnectionString}', IsActive={IsActive} [{RequestId}]", 
                        tenant.ConnectionString?.Substring(0, Math.Min(50, tenant.ConnectionString.Length )) + "...", 
                        1, requestId);
                }
                else
                {
                    _logger.LogWarning("❌ Tenant não encontrado: '{TenantId}' [{RequestId}]", tenantId, requestId);
                    
                    // ✅ LISTAR TENANTS DISPONÍVEIS PARA DEBUG
                    await LogAvailableTenantsAsync(requestId);
                }

                return tenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERRO ao resolver tenant '{TenantId}' [{RequestId}]", tenantId, requestId);
                _logger.LogError("Exception Details: Type={ExceptionType}, Message={Message} [{RequestId}]", 
                    ex.GetType().Name, ex.Message, requestId);
                throw;
            }
        }

        public void SetCurrentTenant(Tenant tenant)
        {
            try
            {
                _logger.LogInformation("=== SetCurrentTenant ===");
                
                if (tenant == null)
                {
                    _logger.LogWarning("Tentativa de definir tenant NULL como atual");
                    _currentTenant = null;
                    return;
                }

                _logger.LogInformation("Definindo tenant atual: ID='{TenantId}', Name='{TenantName}'", 
                    tenant.Id, tenant.Name);
                _logger.LogDebug("Tenant atual anterior: {PreviousTenant}", _currentTenant?.Id ?? "NULL");
                
                _currentTenant = tenant;
                
                _logger.LogInformation("✅ Tenant atual definido com sucesso: '{TenantId}'", tenant.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERRO ao definir tenant atual");
                throw;
            }
        }

        public Tenant GetCurrentTenant()
        {
            try
            {
                _logger.LogDebug("=== GetCurrentTenant ===");
                
                if (_currentTenant == null)
                {
                    _logger.LogError("❌ Nenhum tenant foi definido como atual");
                    throw new Exception("Nenhum Tenant foi definido.");
                }

                _logger.LogDebug("✅ Retornando tenant atual: ID='{TenantId}', Name='{TenantName}'", 
                    _currentTenant.Id, _currentTenant.Name);
                
                return _currentTenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERRO ao obter tenant atual");
                throw;
            }
        }

        public async Task CreateTenantAsync(Tenant tenant)
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            try
            {
                _logger.LogInformation("=== CreateTenantAsync [{RequestId}] ===", requestId);
                _logger.LogInformation("Criando tenant: ID='{TenantId}', Name='{TenantName}', Database='{DatabaseName}' [{RequestId}]", 
                    tenant.Id, tenant.Name, tenant.DatabaseName, requestId);

                var defaultConnectionString = _configuration.GetConnectionString("DefaultConnection");
                _logger.LogDebug("Connection string padrão: {ConnectionString} [{RequestId}]", 
                    defaultConnectionString?.Substring(0, Math.Min(50, defaultConnectionString?.Length ?? 0)) + "...", requestId);
                
                var builder = new SqlConnectionStringBuilder(defaultConnectionString)
                {
                    InitialCatalog = tenant.DatabaseName
                };

                _logger.LogInformation("Connection string do tenant: {TenantConnectionString} [{RequestId}]", 
                    builder.ToString().Substring(0, Math.Min(80, builder.ToString().Length)) + "...", requestId);

                var configuration = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();

                SetCurrentTenant(tenant);
                
                // Criar banco e executar migrações
                _logger.LogInformation("Criando banco de dados para o tenant [{RequestId}]", requestId);
                await CreateDatabaseAsync(builder.ToString());
                
                var tenantService = new TenantService(_tenantRepository, configuration, _tenantContext, _currentTenant, _logger);

                _logger.LogInformation("Executando migrações para o tenant [{RequestId}]", requestId);
                await ExecuteMigrationsAsync(builder.ToString(), configuration, tenantService);

                _logger.LogInformation("Salvando tenant no contexto [{RequestId}]", requestId);
                await _tenantContext.Tenants.AddAsync(tenant);
                await _tenantContext.SaveChangesAsync();
                
                _logger.LogInformation("✅ Tenant criado com sucesso: '{TenantId}' [{RequestId}]", tenant.Id, requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERRO ao criar tenant '{TenantId}' [{RequestId}]", tenant?.Id ?? "NULL", requestId);
                throw;
            }
        }

        private async Task CreateDatabaseAsync(string connectionString)
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            try
            {
                _logger.LogInformation("=== CreateDatabaseAsync [{RequestId}] ===", requestId);
                
                var databaseName = new SqlConnectionStringBuilder(connectionString).InitialCatalog;
                _logger.LogInformation("Criando banco de dados: '{DatabaseName}' [{RequestId}]", databaseName, requestId);

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                _logger.LogDebug("Abrindo conexão com servidor [{RequestId}]", requestId);
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandText = $"CREATE DATABASE [{databaseName}]";
                _logger.LogDebug("Executando comando SQL: {SqlCommand} [{RequestId}]", command.CommandText, requestId);
                
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("✅ Banco de dados '{DatabaseName}' criado com sucesso [{RequestId}]", databaseName, requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERRO ao criar banco de dados [{RequestId}]", requestId);
                throw;
            }
        }

        public async Task ExecuteMigrationsAsync(string connectionString, IConfiguration configuration, ITenantService tenantService)
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            try
            {
                _logger.LogInformation("=== ExecuteMigrationsAsync [{RequestId}] ===", requestId);
                _logger.LogDebug("Connection string para migrations: {ConnectionString} [{RequestId}]", 
                    connectionString.Substring(0, Math.Min(80, connectionString.Length)) + "...", requestId);

                var optionsBuilder = new DbContextOptionsBuilder<GhotelDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                _logger.LogDebug("Criando contexto para migrations [{RequestId}]", requestId);
                using var context = new GhotelDbContext(optionsBuilder.Options, tenantService, configuration);
                
                _logger.LogInformation("Executando migrações do banco de dados [{RequestId}]", requestId);
                await context.Database.MigrateAsync();
                
                _logger.LogInformation("✅ Migrações executadas com sucesso [{RequestId}]", requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERRO ao executar migrações [{RequestId}]", requestId);
                throw;
            }
        }

        public async Task CreateTenantDatabaseAsync(Tenant tenant)
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            try
            {
                _logger.LogInformation("=== CreateTenantDatabaseAsync [{RequestId}] ===", requestId);
                _logger.LogInformation("Criando banco para tenant: ID='{TenantId}', Database='{DatabaseName}' [{RequestId}]", 
                    tenant.Id, tenant.DatabaseName, requestId);

                var connectionString = _configuration.GetConnectionString("Default");
                var builder = new SqlConnectionStringBuilder(connectionString)
                {
                    InitialCatalog = tenant.DatabaseName
                };

                _logger.LogDebug("Connection string construída: {ConnectionString} [{RequestId}]", 
                    builder.ToString().Substring(0, Math.Min(80, builder.ToString().Length)) + "...", requestId);

                using var connection = new SqlConnection(builder.ToString());
                _logger.LogDebug("Abrindo conexão [{RequestId}]", requestId);
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandText = $"CREATE DATABASE [{tenant.DatabaseName}]";
                _logger.LogDebug("Executando: {SqlCommand} [{RequestId}]", command.CommandText, requestId);
                
                await command.ExecuteNonQueryAsync();
                _logger.LogInformation("✅ Banco '{DatabaseName}' criado com sucesso [{RequestId}]", tenant.DatabaseName, requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERRO ao criar banco do tenant '{TenantId}' [{RequestId}]", tenant?.Id ?? "NULL", requestId);
                throw;
            }
        }

        public async Task<Tenant> UpdateAsync(Tenant tenant)
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            try
            {
                _logger.LogInformation("=== UpdateAsync [{RequestId}] ===", requestId);
                _logger.LogInformation("Atualizando tenant: ID='{TenantId}', Name='{TenantName}' [{RequestId}]", 
                    tenant.Id, tenant.Name, requestId);

                var existing = await _tenantContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenant.Id);
                
                if (existing != null)
                {
                    _logger.LogDebug("Tenant existente encontrado: ID='{ExistingId}' [{RequestId}]", existing.Id, requestId);
                    _logger.LogDebug("Removendo tenant existente [{RequestId}]", requestId);
                    _tenantContext.Remove(existing);
                    
                    _logger.LogDebug("Adicionando tenant atualizado [{RequestId}]", requestId);
                    _tenantContext.Update(tenant);
                    
                    _logger.LogDebug("Salvando alterações [{RequestId}]", requestId);
                    await _tenantContext.SaveChangesAsync();
                    
                    _logger.LogInformation("✅ Tenant atualizado com sucesso: '{TenantId}' [{RequestId}]", tenant.Id, requestId);
                }
                else
                {
                    _logger.LogWarning("❌ Tenant não encontrado para atualização: '{TenantId}' [{RequestId}]", tenant.Id, requestId);
                }

                return tenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERRO ao atualizar tenant '{TenantId}' [{RequestId}]", tenant?.Id ?? "NULL", requestId);
                throw;
            }
        }

        public Task DeleteAsync(string id)
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            try
            {
                _logger.LogInformation("=== DeleteAsync [{RequestId}] ===", requestId);
                _logger.LogInformation("Deletando tenant: ID='{TenantId}' [{RequestId}]", id, requestId);

                var tenant = _tenantContext.Tenants.FirstOrDefault(t => t.Id == id);
                
                if (tenant != null)
                {
                    _logger.LogDebug("Tenant encontrado para deleção: ID='{TenantId}', Name='{TenantName}' [{RequestId}]", 
                        tenant.Id, tenant.Name, requestId);
                    _tenantContext.Remove(tenant);
                    _logger.LogInformation("✅ Tenant removido: '{TenantId}' [{RequestId}]", id, requestId);
                }
                else
                {
                    _logger.LogWarning("❌ Tenant não encontrado para deleção: '{TenantId}' [{RequestId}]", id, requestId);
                }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERRO ao deletar tenant '{TenantId}' [{RequestId}]", id, requestId);
                throw;
            }
        }

        public Task<IEnumerable<Tenant>> GetAllAsync()
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            try
            {
                _logger.LogInformation("=== GetAllAsync [{RequestId}] ===", requestId);
                
                var tenants = _tenantContext.Tenants.AsEnumerable();
                var tenantCount = tenants.Count();
                
                _logger.LogInformation("Retornando {TenantCount} tenants [{RequestId}]", tenantCount, requestId);
                
                if (tenantCount > 0)
                {
                    _logger.LogDebug("Tenants encontrados: [{TenantList}] [{RequestId}]", 
                        string.Join(", ", tenants.Select(t => $"{t.Id}({t.Name})")), requestId);
                }
                else
                {
                    _logger.LogWarning("❌ Nenhum tenant encontrado no sistema [{RequestId}]", requestId);
                }

                return Task.FromResult(tenants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERRO ao obter todos os tenants [{RequestId}]", requestId);
                throw;
            }
        }

        public Task<Tenant> GetByIdAsync(string id)
        {
            var requestId = Guid.NewGuid().ToString("N")[..8];
            
            try
            {
                _logger.LogInformation("=== GetByIdAsync [{RequestId}] ===", requestId);
                _logger.LogInformation("Buscando tenant por ID: '{TenantId}' [{RequestId}]", id, requestId);

                var tenant = _tenantContext.Tenants.FirstOrDefault(t => t.Id == id);
                
                if (tenant != null)
                {
                    _logger.LogInformation("✅ Tenant encontrado: ID='{TenantId}', Name='{TenantName}' [{RequestId}]", 
                        tenant.Id, tenant.Name, requestId);
                }
                else
                {
                    _logger.LogWarning("❌ Tenant não encontrado: '{TenantId}' [{RequestId}]", id, requestId);
                    
                    // ✅ LISTAR TENANTS DISPONÍVEIS PARA DEBUG
                    LogAvailableTenantsSync(requestId);
                }

                return Task.FromResult(tenant);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERRO ao buscar tenant por ID '{TenantId}' [{RequestId}]", id, requestId);
                throw;
            }
        }

        /// <summary>
        /// ✅ LISTAR TENANTS DISPONÍVEIS PARA DEBUG (ASYNC)
        /// </summary>
        private async Task LogAvailableTenantsAsync(string requestId)
        {
            try
            {
                _logger.LogInformation("=== Listando tenants disponíveis (Debug) [{RequestId}] ===", requestId);
                
                var allTenants = await GetAllAsync();
                
                if (allTenants?.Any() == true)
                {
                    _logger.LogInformation("Tenants disponíveis no sistema ({Count}) [{RequestId}]:", allTenants.Count(), requestId);
                    
                    foreach (var t in allTenants)
                    {
                        _logger.LogInformation("  - ID: '{Id}', Name: '{Name}', Database: '{Database}', Active: {IsActive} [{RequestId}]", 
                            t.Id, t.Name, t.DatabaseName, 1,  requestId);
                    }
                }
                else
                {
                    _logger.LogWarning("❌ NENHUM tenant encontrado no sistema [{RequestId}]", requestId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar tenants disponíveis [{RequestId}]", requestId);
            }
        }

        /// <summary>
        /// ✅ LISTAR TENANTS DISPONÍVEIS PARA DEBUG (SYNC)
        /// </summary>
        private void LogAvailableTenantsSync(string requestId)
        {
            try
            {
                _logger.LogInformation("=== Listando tenants disponíveis (Debug Sync) [{RequestId}] ===", requestId);
                
                var allTenants = _tenantContext.Tenants.AsEnumerable();
                
                if (allTenants?.Any() == true)
                {
                    _logger.LogInformation("Tenants disponíveis no sistema ({Count}) [{RequestId}]:", allTenants.Count(), requestId);
                    
                    foreach (var t in allTenants)
                    {
                        _logger.LogInformation("  - ID: '{Id}', Name: '{Name}', Database: '{Database}', Active: {IsActive} [{RequestId}]", 
                            t.Id, t.Name, t.DatabaseName, 1, requestId);
                    }
                }
                else
                {
                    _logger.LogWarning("❌ NENHUM tenant encontrado no sistema [{RequestId}]", requestId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar tenants disponíveis sync [{RequestId}]", requestId);
            }
        }
    }
}