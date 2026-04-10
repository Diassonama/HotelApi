using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class EmpresaSaldoMovimentoConfiguration: IEntityTypeConfiguration<EmpresaSaldoMovimento>
    {
        public void Configure(EntityTypeBuilder<EmpresaSaldoMovimento> builder)
        {

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Valor)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();


          /*   builder.Property(e => e.TipoLancamento)
             .HasConversion<string>() // Conversão automática do EF Core
                .HasColumnType("varchar(1)")
                .HasDefaultValue(Domain.Enums.TipoLancamento.E);
 */

/*             builder.Property(c => c.TipoLancamentoId)
                .HasConversion<string>() // Conversão automática do EF Core
                .HasColumnType("varchar(1)")
                .HasDefaultValue(Domain.Enums.TipoLancamento.E); */

            builder.Property(e => e.Documento)
                   .HasMaxLength(50);

            builder.Property(e => e.Observacao)
                   .HasMaxLength(500);

            builder.HasOne(e => e.EmpresaSaldo)
                   .WithMany(es => es.EmpresaSaldoMovimentos)
                   .HasForeignKey(e => e.EmpresaSaldoId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Utilizador)
                   .WithMany()
                   .HasForeignKey(e => e.UtilizadorId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.EmpresaSaldoId);

        }
        
    }
}