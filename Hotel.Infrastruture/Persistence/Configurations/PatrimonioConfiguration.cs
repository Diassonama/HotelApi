using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class PatrimonioConfiguration : IEntityTypeConfiguration<Patrimonio>
    {
        public void Configure(EntityTypeBuilder<Patrimonio> builder)
        {
            builder.HasKey(p => p.Id);
           /*  builder.HasMany(o=>o.MobiliaApartamentos).WithOne(o=>o.Patrimonios);
            builder.HasMany(o=>o.MobiliaTipoApartamentos).WithMany(o=>o.patrimonios); */
        }
    }
}