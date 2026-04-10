using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class TaxTableEntryConfiguration : IEntityTypeConfiguration<TaxTableEntry>
    {
        public void Configure(EntityTypeBuilder<TaxTableEntry> builder)
        {
           builder.HasKey(p=> p.Id);
           builder.HasOne(p=>p.TaxTypes).WithMany(o=>o.TaxTableEntry).HasForeignKey(t=> t.TaxType);
         //  builder.HasOne(p=>p.Tax).WithMany(o=>o.TaxTableEntry).HasForeignKey(t=> t.TaxCode);
         //  builder.HasMany(p=>p.TaxExemptionReasons).WithOne(p=>p.TaxTableEntry);
         //  builder.HasMany(p=>p.Produtos).WithOne(p=>p.TaxTableEntry);
        }
    }
}