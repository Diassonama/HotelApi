using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class TaxTypesConfiguration : IEntityTypeConfiguration<TaxTypes>
    {
        public void Configure(EntityTypeBuilder<TaxTypes> builder)
        {
            builder.HasKey(p => p.TaxType);
            builder.Property(p => p.TaxType).HasMaxLength(3);
            builder.Property(p => p.Descricao).HasMaxLength(50);

/*             builder.HasMany(p => p.TaxTableEntry)
                .WithOne(p => p.TaxTypes)
                .IsRequired(); */
        }
    }
}