using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class FacturaEmpresaConfiguration : IEntityTypeConfiguration<FacturaEmpresa>
    {
        public void Configure(EntityTypeBuilder<FacturaEmpresa> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Empresas)
                   .WithMany(p => p.FacturaEmpresas)
                   .HasForeignKey(f => f.EmpresasId);

      /*       builder.HasOne(p => p.checkins)
                    .WithMany(p => p.FacturaEmpresas)
                    .HasForeignKey(f => f.CheckinsId); */

            builder.Property(c => c.SituacaoFacturas);
            builder.Property(c => c.SituacaoFacturas)
                .HasConversion<string>();


/*        
        builder.HasMany(f => f.Checkins)  // Uma FacturaEmpresa pode ter vários Checkins
        .WithOne(c => c.FacturaEmpresas)  // Um Checkin pertence a uma única FacturaEmpresa
        .HasForeignKey(c => c.FacturaEmpresasId)  // FK no Checkins
        .OnDelete(DeleteBehavior.Cascade);  */ 
        }
    }
}