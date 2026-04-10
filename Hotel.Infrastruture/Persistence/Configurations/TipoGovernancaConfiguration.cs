using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class TipoGovernancaConfiguration : IEntityTypeConfiguration<TipoGovernanca>
    {
        public void Configure(EntityTypeBuilder<TipoGovernanca> builder)
        {
          builder.HasKey(p => p.Id);
          //  builder.HasMany(p=>p.TipoApartamentos).WithOne(p=>p.T)
          //  builder.HasMany(p=>p.Governancas).WithOne(p=>p.TipoGovernancas);

              builder.HasData(
                            new TipoGovernanca { Id = 1, Descricao = "Arrumação" },
                            new TipoGovernanca { Id = 2, Descricao = "Limpeza" },
                            new TipoGovernanca { Id = 3, Descricao = "Sujo"},
                            new TipoGovernanca { Id = 4, Descricao = "Manutenção" }
                            ); 
        }

        
    }
}