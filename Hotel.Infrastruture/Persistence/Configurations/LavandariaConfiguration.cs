using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class LavandariaConfiguration : IEntityTypeConfiguration<Lavandaria>
    {
        public void Configure(EntityTypeBuilder<Lavandaria> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(z => z.Utilizadores).WithMany(p => p.Lavandarias).IsRequired();
            builder.HasOne(z => z.Clientes).WithMany(p => p.Lavandarias).IsRequired();

        }
    }
}