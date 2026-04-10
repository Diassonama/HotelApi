using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class TaxConfiguration : IEntityTypeConfiguration<Tax>
    {
        public void Configure(EntityTypeBuilder<Tax> builder)
        {
            builder.HasKey(a=> a.Id);
            builder.HasOne(o=>o.TaxTypes).WithMany(o=>o.Tax).HasForeignKey(a=> a.TaxType);

            builder.Property(p=>p.TaxCode).HasMaxLength(3);
           

        }
    }
}