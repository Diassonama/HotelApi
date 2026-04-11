using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.SituacaoPagamento).HasConversion<string>();

            // DB column Valor is real (SQL Server 4-byte float = System.Single) — convert to/from decimal
            builder.Property(p => p.Valor)
                .HasConversion(
                    v => (float)v,
                    v => (decimal)v
                )
                .HasColumnType("real");

            builder.HasOne(p => p.PontoVenda)
                .WithMany(p => p.Pedidos)
                .IsRequired();

            builder.HasOne(p => p.Hospede)
              .WithMany(p => p.Pedidos)
              .IsRequired();
        }
    }
}