using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class TipoHospedagemConfiguration : IEntityTypeConfiguration<TipoHospedagem>
    {
        public void Configure(EntityTypeBuilder<TipoHospedagem> builder)
        {
            builder.HasKey(p => p.Id);
            
            // Configuração da propriedade decimal
            builder.Property(p => p.Valor)
                   .HasColumnType("decimal(18,2)");
                   
           // builder.HasMany(p=>p.Diarias).WithOne(p=>p.TipoHospedagens);

           builder.HasData(
           
                new TipoHospedagem { Id = 1, Descricao = "DIARIA", Valor =0 },
                new TipoHospedagem { Id = 2, Descricao = "HORA", Valor =0 },
                new TipoHospedagem { Id = 3, Descricao = "NOITE", Valor =0 },
                new TipoHospedagem { Id = 4, Descricao = "ESPECIAL", Valor =0 },
                new TipoHospedagem { Id = 5, Descricao = "DIARIA(PA)", Valor =15000 },
                new TipoHospedagem { Id = 6, Descricao = "MENSAL", Valor =0 },
                new TipoHospedagem { Id = 7, Descricao = "CAMA EXTRA", Valor =0 },
                new TipoHospedagem { Id = 8, Descricao = "DIARIA(SPA)", Valor =10000 }
        
           );

        }
    }
}