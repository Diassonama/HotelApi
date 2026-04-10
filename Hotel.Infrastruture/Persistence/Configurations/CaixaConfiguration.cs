using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class CaixaConfiguration : IEntityTypeConfiguration<Caixa>
    {
        public void Configure(EntityTypeBuilder<Caixa> builder)
        {
           builder.HasKey(p=>p.Id);
           builder.HasOne(p=> p.Utilizadores).WithMany(p=> p.Caixas).HasForeignKey(m=>m.UtilizadoresId);
           //transforme a DataDeFechamento para ser opcional, pois o caixa pode estar aberto
           builder.Property(p => p.DataDeFechamento).IsRequired(false);
          /*  builder.HasData(
               new Caixa { Id = 1, SaldoInicial = 0,SaldoFinal= 0, SaldoAtual= 0,DataDeAbertura=DateTime.Now,DataDeFechamento=DateTime.Now ,Entrada = 0, Saida = 0,SaldoPendenteCaixaAnterior=0,SaldoPendeteCaixaAtual=0, UtilizadoresId = "b609ca9d-79a6-416c-84d6-33b1c382686a" }
           ); */
        }
    }
}