using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class LancamentoCaixaConfiguration : IEntityTypeConfiguration<LancamentoCaixa>
    {
        public void Configure(EntityTypeBuilder<LancamentoCaixa> builder)
        {
            builder.HasKey(p => p.Id);
            
            // Relacionamento com Caixas
            builder.HasOne(p => p.Caixas)
                   .WithMany(p => p.LancamentoCaixas)
                   .HasForeignKey(m => m.CaixasId)
                   .OnDelete(DeleteBehavior.Restrict);
            
            // Relacionamento com TipoPagamentos
            builder.HasOne(p => p.TipoPagamentos)
                   .WithMany(p => p.LancamentoCaixas)
                   .HasForeignKey(m => m.TipoPagamentosId)
                   .OnDelete(DeleteBehavior.Restrict);
            
            // Relacionamento com Pagamentos
            builder.HasOne(p => p.Pagamentos)
                   .WithMany(p => p.LancamentoCaixas)
                   .HasForeignKey(m => m.PagamentosId)
                   .OnDelete(DeleteBehavior.Restrict);
            
            // Relacionamento com PlanoDeConta
            builder.HasOne(p => p.PlanodeContas)
                   .WithMany(p => p.LancamentoCaixas)
                   .HasForeignKey(m => m.PlanoDeContasId)
                   .OnDelete(DeleteBehavior.Restrict);       

            // Relacionamento com Utilizadores
            builder.HasOne(p => p.Utilizadores)
                   .WithMany(p => p.LancamentoCaixas)
                   .HasForeignKey(m => m.UtilizadoresId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
            // Configuração do enum TipoLancamento
            builder.Property(c => c.TipoLancamento)
                   .HasConversion<string>()
                   .IsRequired();

                     // Configurações adicionais de propriedades
                     builder.Property(p => p.Valor)
                            .HasColumnType("decimal(18,2)")
                            .IsRequired(); 
                    /*  builder.Property(p => p.Valor)
                   .HasColumnType("float")
                   .IsRequired(); */

            builder.Property(p => p.Observacao)
                   .HasMaxLength(500);

            builder.Property(p => p.DataHoraLancamento)
                   .IsRequired();
        }
    }
}