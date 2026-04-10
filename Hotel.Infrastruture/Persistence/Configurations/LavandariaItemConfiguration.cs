using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class LavandariaItemConfiguration : IEntityTypeConfiguration<LavandariaItem>
    {
        public void Configure(EntityTypeBuilder<LavandariaItem> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p=>p.Lavandarias).WithMany(p=>p.LavandariaItems);
            builder.HasOne(p=>p.Utilizadores).WithMany(p=>p.LavandariaItems);
        }
    }
}