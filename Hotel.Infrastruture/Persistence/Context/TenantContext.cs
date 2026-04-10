using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Hotel.Domain.Tenant.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hotel.Infrastruture.Persistence.Context
{
    public class TenantContext : DbContext
    {
        readonly IConfiguration _configuration;
        
        public TenantContext()
        {
            // Construtor padrão necessário para migrations
        }
        
        public TenantContext(DbContextOptions<TenantContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Se já foi configurado via DI, não reconfigure
            if (optionsBuilder.IsConfigured)
                return;

            // Primeira prioridade: usar configuração se disponível
            if (_configuration != null)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection") 
                                    ?? _configuration.GetConnectionString("TenantConnection");
                
                if (!string.IsNullOrEmpty(connectionString))
                {
                    optionsBuilder.UseSqlServer(connectionString);
                    optionsBuilder.EnableSensitiveDataLogging();
                    return;
                }
            }

            // Fallback: connection string hardcoded para migrations
            var fallbackConnectionString = "Server=SQL1004.site4now.net;Database=db_abc40d_tenant;User Id=db_abc40d_tenant_admin;Password=RENT2024;Encrypt=false;";
            
            optionsBuilder.UseSqlServer(fallbackConnectionString);
            optionsBuilder.EnableSensitiveDataLogging();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Tenant>().ToTable("Tenant");
            
            modelBuilder.Entity<Tenant>()
                .OwnsOne(t => t.Metadata, metadata =>
                {
                    metadata.Property(m => m.Region).HasMaxLength(100);
                    metadata.Property(m => m.MaxUsers).IsRequired();
                    metadata.Property(m => m.IsActive).HasDefaultValue(true);
                    
                    metadata.Property(m => m.CustomSettings)
                        .HasConversion(
                            v => JsonSerializer.Serialize(v, new JsonSerializerOptions { WriteIndented = false }),
                            v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Dictionary<string, string>()
                        )
                        .HasColumnType("nvarchar(max)");
                });
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}