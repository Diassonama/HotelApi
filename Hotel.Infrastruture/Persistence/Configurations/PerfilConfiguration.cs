using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class PerfilConfiguration : IEntityTypeConfiguration<Perfil>
    {
        public void Configure(EntityTypeBuilder<Perfil> builder)
        {
            builder.HasKey(p => p.Id);
          //  builder.HasKey(p=>p.Id);
          // builder.HasMany(p=> p.Acessos).WithOne(p=> p.Perfils);
         //   builder.HasMany(p=> p.Utilizadores).WithOne(p=> p.Perfis);
         builder.HasData(
             new Perfil { Id = 1, Descricao = "Administrador" }
         );

         
        }
    }
}