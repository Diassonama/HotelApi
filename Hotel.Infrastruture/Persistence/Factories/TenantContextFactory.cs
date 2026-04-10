using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Hotel.Infrastruture.Persistence.Context;

namespace Hotel.Infrastruture.Persistence.Factories
{
    public class TenantContextFactory : IDesignTimeDbContextFactory<TenantContext>
    {
        public TenantContext CreateDbContext(string[] args)
        {
            // Carregar configuração do appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<TenantContext>();

            // Configurar a conexão
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                   ?? "Server=SQL1004.site4now.net;Database=db_abc40d_tenant;User Id=db_abc40d_tenant_admin;Password=RENT2024;Encrypt=false;";

            optionsBuilder.UseSqlServer(connectionString);

            return new TenantContext(optionsBuilder.Options, configuration);
        }
    }
}