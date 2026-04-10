using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produtos>
    {
        public void Configure(EntityTypeBuilder<Produtos> builder)
        {
            // ✅ CONFIGURAÇÃO DA CHAVE PRIMÁRIA
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            // ✅ PROPRIEDADES OBRIGATÓRIAS COM TIPOS CORRETOS (FLOAT)
            builder.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Valor)
                .IsRequired()
                .HasColumnType("real"); // ✅ REAL = FLOAT no SQL Server

            builder.Property(e => e.PrecoCompra)
                .IsRequired()
                .HasColumnType("real"); // ✅ REAL = FLOAT no SQL Server

            builder.Property(e => e.PrecoCIva)
                .IsRequired()
                .HasColumnType("real") // ✅ REAL = FLOAT no SQL Server
                .HasDefaultValue(0f);

            builder.Property(e => e.ValorFixo)
                .IsRequired()
                .HasColumnType("real") // ✅ REAL = FLOAT no SQL Server
                .HasDefaultValue(0f);

            builder.Property(e => e.Lucro)
                .IsRequired()
                .HasColumnType("real") // ✅ REAL = FLOAT no SQL Server
                .HasDefaultValue(0f);

            builder.Property(e => e.MargemLucro)
                .IsRequired()
                .HasColumnType("real") // ✅ REAL = FLOAT no SQL Server
                .HasDefaultValue(0f);

            builder.Property(e => e.Desconto)
                .IsRequired()
                .HasColumnType("real") // ✅ REAL = FLOAT no SQL Server
                .HasDefaultValue(0f);

            builder.Property(e => e.DescontoPercentagem)
                .IsRequired()
                .HasColumnType("real") // ✅ REAL = FLOAT no SQL Server
                .HasDefaultValue(0f);

            builder.Property(e => e.Quantidade)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.EstoqueMinimo)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.AdicionarStock)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.CategoriaId)
                .IsRequired();

            builder.Property(e => e.PontoDeVendasId)
                .IsRequired();

            builder.Property(e => e.DataExpiracao)
                .IsRequired();

            // ✅ CONFIGURAÇÃO DO TaxTableEntryId - OBRIGATÓRIO
            builder.Property(p => p.TaxTableEntryId)
                .HasColumnName("TaxTableEntryId")
                .HasColumnType("int")
                .IsRequired(true)
                .HasDefaultValue(1)
                .ValueGeneratedNever(); // ✅ FORÇA INCLUSÃO NO INSERT

            // ✅ PROPRIEDADES DE CÓDIGOS FISCAIS
            builder.Property(e => e.ProductTypeCode)
                .HasMaxLength(1)
                .IsRequired()
                .HasDefaultValue("P");

            builder.Property(e => e.TaxExemptionReasonCode)
                .HasMaxLength(3)
                .IsRequired()
                .HasDefaultValue("M00");

            builder.Property(e => e.CaminhoImagem)
                .HasMaxLength(500)
                .IsRequired(false);

            // ✅ RELACIONAMENTOS
            builder.HasOne(e => e.Categoria)
                .WithMany(c => c.Produtos)
                .HasForeignKey(e => e.CategoriaId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.PontoDeVenda)
                .WithMany(pv => pv.Produtos)
                .HasForeignKey(e => e.PontoDeVendasId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.TaxTableEntry)
                .WithMany(o => o.Produtos)
                .HasForeignKey(o => o.TaxTableEntryId)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.ProductTypes)
                .WithMany(o => o.Produtos)
                .HasForeignKey(o => o.ProductTypeCode)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.TaxExemptionReason)
                .WithMany(o => o.Produtos)
                .HasForeignKey(o => o.TaxExemptionReasonCode)
                .IsRequired(true)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.ProdutoStocks)
                .WithOne(ps => ps.Produto)
                .HasForeignKey(ps => ps.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}