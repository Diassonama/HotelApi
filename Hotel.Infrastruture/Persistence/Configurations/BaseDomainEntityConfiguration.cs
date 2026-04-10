using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class BaseDomainEntityConfiguration : IEntityTypeConfiguration<BaseDomainEntity>
    {
        public void Configure(EntityTypeBuilder<BaseDomainEntity> builder)
        {
           // builder.Property(p=> p.DateCreated).HasDefaultValue("getdate()");
            builder.HasKey(o=>o.Id);
            builder.Property(p=>p.Id).HasDefaultValueSql("NEWID()");
            builder.Property(p=> p.IsActive).HasDefaultValue(false);
        }
    }
}