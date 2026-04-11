using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
    {
        public void Configure(EntityTypeBuilder<ItemPedido> builder)
        {
            builder.HasKey(p => p.Id);

            // DB column is real (SQL Server 4-byte float = System.Single) — convert to/from decimal transparently
            builder.Property(p => p.Preco)
                .HasConversion(
                    v => (float)v,
                    v => (decimal)v
                )
                .HasColumnType("real");
        }
    }
}
