using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
    {
        public void Configure(EntityTypeBuilder<Pagamento> builder)
        {
            builder.HasKey(p => p.Id);
         // Origem e OrigemId - campos para rastreamento genérico da fonte do pagamento
            builder.Property(p => p.Origem)
                .HasMaxLength(50)
                .IsRequired();
            
            builder.Property(p => p.OrigemId)
                .IsRequired();

            builder.Property(p => p.Observacao)
                .HasMaxLength(500);

            // Relacionamento com Hospede
            builder.HasOne(p => p.Hospedes)
                .WithMany(p => p.Pagamentos)
                .HasForeignKey(m => m.HospedesId)
                .OnDelete(DeleteBehavior.NoAction);

            // Relacionamento com Utilizador
            builder.HasOne(p => p.Utilizadores)
                .WithMany(p => p.Pagamentos)
                .HasForeignKey(p => p.UtilizadoresId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.Property(e => e.Status)
                .HasConversion<string>();

            // Índice composto para melhorar performance de buscas por origem
            builder.HasIndex(p => new { p.Origem, p.OrigemId })
                .HasDatabaseName("IX_Pagamento_Origem_OrigemId");

            // Índice para buscar pagamentos por hóspede
            builder.HasIndex(p => p.HospedesId)
                .HasDatabaseName("IX_Pagamento_HospedesId");


        }
    }
}