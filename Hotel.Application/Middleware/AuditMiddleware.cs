using System;
using System.Threading.Tasks;
using Serilog;
using Hotel.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Hotel.Application.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            try
            {
                // Obtém o tenant atual
                var tenant = tenantService.GetCurrentTenant();

                if (tenant != null)
                {
                    // Criação do objeto de auditoria
                    var auditInfo = new
                    {
                        TenantId = tenant.Id,
                        Path = context.Request.Path,
                        Method = context.Request.Method,
                        Timestamp = DateTime.UtcNow,
                        UserAgent = context.Request.Headers["User-Agent"].ToString(),
                        IPAddress = context.Connection.RemoteIpAddress?.ToString()
                    };

                    // Registra a auditoria no log
                    Log.Information("Audit Log: {@AuditInfo}", auditInfo);
                }
                else
                {
                    Log.Warning("Audit Log: Nenhum Tenant encontrado para a requisição.");
                }

                // Continua o fluxo da requisição
                await _next(context);
            }
            catch (Exception ex) when (ex.Message.Contains("Unexpected end of request content") || 
                                         ex.GetType().Name.Contains("BadHttpRequestException"))
            {
                // Log específico para erros de HTTP malformados
                Log.Warning(ex, "Erro de requisição HTTP malformada no AuditMiddleware. Path: {Path}, Method: {Method}", 
                    context.Request.Path, context.Request.Method);
                
                // Retorna erro 400 Bad Request para requisições malformadas
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Bad Request: Request content is malformed or incomplete.");
                return;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao processar auditoria no AuditMiddleware.");
                throw; // Repropaga a exceção para não mascarar outros erros
            }
        }
    }
}
