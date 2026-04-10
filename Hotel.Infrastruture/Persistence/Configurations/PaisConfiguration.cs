using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class PaisConfiguration : IEntityTypeConfiguration<Pais>
    {
        public void Configure(EntityTypeBuilder<Pais> builder)
        {
            builder.HasKey(p => p.Id);
         //   builder.HasMany(o=>o.Clientes).WithOne(o=>o.Paises);
         builder.HasData(
             new Pais { Id = 1, Nome = "Brasil" },
             new Pais { Id = 2, Nome = "Estados Unidos" },
             new Pais { Id = 3, Nome = "Canadá" },
             new Pais { Id = 4, Nome = "Argentina" },
             new Pais { Id = 5, Nome = "Chile" },
             new Pais { Id = 6, Nome = "Uruguai" },
             new Pais { Id = 7, Nome = "Paraguai" },
             new Pais { Id = 8, Nome = "Bolívia" },
             new Pais { Id = 9, Nome = "Peru" },
             new Pais { Id = 10, Nome = "Equador" },
             new Pais { Id = 11, Nome = "Colômbia" },
             new Pais { Id = 12, Nome = "Venezuela" },
             new Pais { Id = 13, Nome = "Guiana" },
             new Pais { Id = 14, Nome = "Suriname" },
             new Pais { Id = 15, Nome = "França" },
             new Pais { Id = 16, Nome = "Espanha" },
             new Pais { Id = 17, Nome = "Itália" },
             new Pais { Id = 18, Nome = "Alemanha" },
             new Pais { Id = 19, Nome = "Reino Unido" },
             new Pais { Id = 20, Nome = "Portugal" },
             new Pais { Id = 21, Nome = "Suíça" },
             new Pais { Id = 22, Nome = "Austrália" },
             new Pais { Id = 23, Nome = "Nova Zelândia" },
             new Pais { Id = 24, Nome = "Japão" },
             new Pais { Id = 25, Nome = "China" },
             new Pais { Id = 26, Nome = "Coreia do Sul" },
             new Pais { Id = 27, Nome = "Índia" },
             new Pais { Id = 28, Nome = "Paquistão" },
             new Pais { Id = 29, Nome = "Bangladesh" },
             new Pais { Id = 30, Nome = "Tailândia" },
             new Pais { Id = 31, Nome = "Filipinas" },
             new Pais { Id = 32, Nome = "Vietnã" },
             new Pais { Id = 33, Nome = "México" },
             new Pais { Id = 34, Nome = "Angola" },
             new Pais { Id = 35, Nome = "Congo Democrático" },
             new Pais { Id = 36, Nome = "Congo" },
             new Pais { Id = 37, Nome = "Moçambique" },
             new Pais { Id = 38, Nome = "Ruanda" }

         );
        }
    }
}