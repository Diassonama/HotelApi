using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class ProdutoStockConfiguration : IEntityTypeConfiguration<ProdutoStock>
    {
        public void Configure(EntityTypeBuilder<ProdutoStock> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p=>p.Produto).WithMany(p=>p.ProdutoStocks);
        }
    }
}