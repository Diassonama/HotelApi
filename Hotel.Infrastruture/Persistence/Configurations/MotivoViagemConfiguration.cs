using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class MotivoViagemConfiguration : IEntityTypeConfiguration<MotivoViagem>
    {
        public void Configure(EntityTypeBuilder<MotivoViagem> builder)
        {
          builder.HasKey(p => p.Id);
          
          builder.HasData(
              new MotivoViagem { Id = 1, Descricao = "Negócios" },
              new MotivoViagem { Id = 2, Descricao = "Lazer" },
              new MotivoViagem { Id = 3, Descricao = "Família" },
              new MotivoViagem { Id = 4, Descricao = "Férias" },
              new MotivoViagem { Id = 5, Descricao = "Estudos" },
              new MotivoViagem { Id = 6, Descricao = "Outros" }
          );
        }
    }
}