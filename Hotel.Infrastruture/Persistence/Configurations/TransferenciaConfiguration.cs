using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class TransferenciaConfiguration : IEntityTypeConfiguration<Transferencia>
    {
        public void Configure(EntityTypeBuilder<Transferencia> builder)
        {
            builder.HasKey(t => t.Id);
            builder.ToTable("Transferencias");

            // ✅ CONFIGURAR COLUNAS EXISTENTES
            builder.Property(t => t.CheckinId)
                .IsRequired()
                .HasColumnName("CheckinId"); // ✅ Confirmar nome da coluna

            builder.Property(t => t.QuartoId)
                .IsRequired()
                .HasColumnName("QuartoId"); // ✅ Confirmar nome da coluna

            builder.Property(t => t.DataEntrada)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(t => t.DataSaida)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(t => t.DataTransferencia)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(t => t.TipoTransferencia)
                .IsRequired()
                .HasConversion<int>(); // Converter enum para int

            builder.Property(t => t.ValorDiaria)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(t => t.Observacao)
                .HasMaxLength(500);

            builder.Property(t => t.UtilizadorId)
                .IsRequired()
                .HasMaxLength(450);

            // ✅ RELACIONAMENTOS
            builder.HasOne(t => t.Checkins)
                .WithMany()
                .HasForeignKey(t => t.CheckinId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Apartamentos)
                .WithMany()
                .HasForeignKey(t => t.QuartoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.MotivoTransferencia)
                .WithMany()
                .HasForeignKey(t => t.MotivoTransferenciaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ IGNORAR PROPRIEDADES QUE NÃO EXISTEM NA TABELA
            // Se houver propriedades calculadas ou de navegação que não são colunas
            // builder.Ignore(t => t.AlgumaPropriedadeCalculada);
        }
    }
}
