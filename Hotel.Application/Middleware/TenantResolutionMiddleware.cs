using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Serilog;


namespace Hotel.Application.Middleware
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger logger;

        public TenantResolutionMiddleware(RequestDelegate next, ILogger logger)
        {
            _next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
        {
            try
            {
                logger.Information("TenantResolutionMiddleware: Iniciando execução...");

                var tenantId = context.Request.Headers["TenantId"].FirstOrDefault()
                                ?? context.Request.Host.Host.Split('.')[0];

                logger.Information($"TenantId identificado: {tenantId}");
             /*    logger.Information($"Host: {context.Request.Host.Host}");
                logger.Information($"Headers: {string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
 */                logger.Information($"TenantId from headers: {context.Request.Headers["TenantId"].FirstOrDefault()}");
                if (tenantId != null)
                {
                    var tenant = await tenantService.ResolveTenantAsync(tenantId);

                    if (tenant != null)
                    {
                        context.Items["Tenant"] = tenant;
                        tenantService.SetCurrentTenant(tenant);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync("Tenant not found.");
                        return;
                    }
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Erro no TenantResolutionMiddleware.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Internal Server Error in Tenant Middleware.");
            }
        }
    }
}

