using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class ProductTypeConfiguration : IEntityTypeConfiguration<ProductType>
    {
        public void Configure(EntityTypeBuilder<ProductType> builder)
        {
           
             builder.HasKey(p => p.ProductTypeCode);
            builder.Property(p => p.ProductTypeCode).HasMaxLength(1);
            
          //  builder.HasMany(p => p.Produtos).WithOne(p => p.ProductTypes).IsRequired();

            builder.HasData(
                new ProductType { ProductTypeCode = "E", ProductTypeDescription = "Imposto Especiais de Consumo - (ex.: IEC)" },
                new ProductType { ProductTypeCode = "I", ProductTypeDescription = "Imposto, taxas e encargos parafiscais " },
                new ProductType { ProductTypeCode = "O", ProductTypeDescription = "Outros (portes debitados, adiantamentos recebidos)" },
                new ProductType { ProductTypeCode = "P", ProductTypeDescription = "Produtos" },
                new ProductType { ProductTypeCode = "S", ProductTypeDescription = "Serviços" }
                ); 
        }
    }
}