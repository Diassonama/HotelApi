using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class ContaConfiguration : IEntityTypeConfiguration<Conta>
    {
        public void Configure(EntityTypeBuilder<Conta> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Descricao).HasMaxLength(60);
          //  builder.HasMany(p => p.PlanoDeContas).WithOne(p => p.Contas);
           
            builder.HasData(
               new Conta { Id = 1,Descricao="ENTRADA/RECEITA" },
               new Conta { Id = 2, Descricao = "SAIDA/DESPESA" }
            );
        }
    }
}