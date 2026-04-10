using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
     public class TipoApartamentoConfiguration : IEntityTypeConfiguration<TipoApartamento>
    {
        public void Configure(EntityTypeBuilder<TipoApartamento> builder)
        {
         // builder.HasKey(p=>p.Id);
         //  builder.Property(o=> o.Id).HasDefaultValueSql("NEXT VALUE FOR Id").ValueGeneratedOnAdd();
         //   builder.Property(x => x.Descricao).HasMaxLength(200).IsRequired();

             /*   builder.HasData(
                    new TipoApartamento { Id = Guid.NewGuid(), Descricao = "Casal Simples" , ValorDiariaSingle= 0,   ValorDiariaDouble = 0 , ValorDiariaTriple= 0, ValorDiariaQuadruple= 0, ValorUmaHora= 0},
                    new TipoApartamento { Id = Guid.NewGuid(), Descricao = "Casal Completo", ValorDiariaSingle= 0,   ValorDiariaDouble = 0 , ValorDiariaTriple= 0, ValorDiariaQuadruple= 0, ValorUmaHora= 0 },
                    new TipoApartamento { Id = Guid.NewGuid(), Descricao = "Duplo(BB)", ValorDiariaSingle= 0,   ValorDiariaDouble = 0 , ValorDiariaTriple= 0, ValorDiariaQuadruple= 0, ValorUmaHora= 0 },
                    new TipoApartamento { Id = Guid.NewGuid(), Descricao = "Quarto Médio", ValorDiariaSingle= 0,   ValorDiariaDouble = 0 , ValorDiariaTriple= 0, ValorDiariaQuadruple= 0, ValorUmaHora= 0 }
            );      */    
               builder.HasData(
                   /*  new TipoApartamento { Id = 1, Descricao = "Casal Simples" , ValorDiariaSingle= 0,   ValorDiariaDouble = 0 , ValorDiariaTriple= 0, ValorDiariaQuadruple= 0, ValorUmaHora= 0},
                    new TipoApartamento { Id = 2, Descricao = "Casal Completo", ValorDiariaSingle= 0,   ValorDiariaDouble = 0 , ValorDiariaTriple= 0, ValorDiariaQuadruple= 0, ValorUmaHora= 0 },
                    new TipoApartamento { Id = 3, Descricao = "Duplo(BB)", ValorDiariaSingle= 0,   ValorDiariaDouble = 0 , ValorDiariaTriple= 0, ValorDiariaQuadruple= 0, ValorUmaHora= 0 },
                    new TipoApartamento { Id = 4, Descricao = "Quarto Médio", ValorDiariaSingle= 0,   ValorDiariaDouble = 0 , ValorDiariaTriple= 0, ValorDiariaQuadruple= 0, ValorUmaHora= 0 },
 */
                    new TipoApartamento(1,"Casal Simples",10000,10000,10000,10000,10000,10000,10000,1000,1000,10000,10000,10000,10000,10000,10000,10000),
                    new TipoApartamento(2,"Casal Completo",10000,10000,10000,10000,10000,10000,10000,1000,1000,10000,10000,10000,10000,10000,10000,10000),
                    new TipoApartamento(3,"Duplo(BB)",10000,10000,10000,10000,10000,10000,10000,1000,1000,10000,10000,10000,10000,10000,10000,10000),
                    new TipoApartamento(4,"Quarto Médio",10000,10000,10000,10000,10000,10000,10000,1000,1000,10000,10000,10000,10000,10000,10000,10000)
                   

            );      
        }
    } 
}