using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
          /*   builder.HasData(
                new Empresa ("Conta Propria", "123456789","Luanda", "hotel1@gmail.com" ),
                new Empresa ("Sonangol", "123456789","Luanda", "hotel1@gmail.com" ),
                new Empresa ("UALA 1", "123456789","Luanda", "hotel1@gmail.com" ),
                 new Empresa { Id = 1, RazaoSocial = "Conta Propria", Endereco = "Luanda", Telefone = "123456789", Email = "hotel1@gmail.com" },
                new Empresa { Id = 2, RazaoSocial = "Sonangol", Endereco = "Luanda", Telefone = "123456789", Email = "hotel1@gmail.com" },
                new Empresa { Id = 3, RazaoSocial = "UALA 1", Endereco = "Luanda", Telefone = "123456789", Email = "hotel1@gmail.com" }
                 ); */
           
        }
    }
}