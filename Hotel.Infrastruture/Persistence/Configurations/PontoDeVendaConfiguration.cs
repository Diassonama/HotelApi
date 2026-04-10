using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class PontoDeVendaConfiguration : IEntityTypeConfiguration<PontoDeVenda>
    {
        public void Configure(EntityTypeBuilder<PontoDeVenda> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.DateCreated).HasDefaultValueSql("getdate()");
             builder.HasData(
                         new PontoDeVenda { Id = 1, Nome = "HOTEL",IsActive = true},
                         new PontoDeVenda { Id = 2, Nome = "LAVANDARIA", IsActive = true },
                         new PontoDeVenda { Id = 3, Nome = "RESTAURANTE", IsActive = true },
                         new PontoDeVenda { Id = 4, Nome = "FRIGOBAR", IsActive = true },
                         new PontoDeVenda { Id = 5, Nome = "BAR", IsActive = true }
                            );   

/*                     builder.HasData(
                         new PontoDeVenda { Id = Guid.NewGuid(), Nome = "HOTEL",IsActive = true},
                         new PontoDeVenda { Id = Guid.NewGuid(), Nome = "LAVANDARIA", IsActive = true },
                         new PontoDeVenda { Id = Guid.NewGuid(), Nome = "RESTAURANTE", IsActive = true },
                         new PontoDeVenda { Id = Guid.NewGuid(), Nome = "FRIGOBAR", IsActive = true },
                         new PontoDeVenda { Id = Guid.NewGuid(), Nome = "BAR", IsActive = true }
                            );  */ 
        }
    }
}