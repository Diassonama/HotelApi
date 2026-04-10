using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class TaxExemptionReasonConfiguration : IEntityTypeConfiguration<TaxExemptionReason>
    {
        public void Configure(EntityTypeBuilder<TaxExemptionReason> builder)
        {
            builder.HasKey(a=> a.TaxExemptionCode);
            builder.HasOne(p=>p.TaxTableEntry).WithMany(p=>p.TaxExemptionReasons).HasForeignKey(a=> a.TaxCode);
            builder.Property(a=> a.TaxExemptionCode).HasMaxLength(3);
            builder.Property(a=> a.TaxExemptionReasons).HasMaxLength(50);
          //  builder.HasMany(o=>o.Produtos).WithOne(p=>p.TaxExemptionReason);
        }
    }
}