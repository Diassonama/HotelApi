using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class MobiliaTipoApartamentoConfiguration : IEntityTypeConfiguration<MobiliaTipoApartamento>
    {
        public void Configure(EntityTypeBuilder<MobiliaTipoApartamento> builder)
        {
            builder.HasKey(p => p.Id);
     //      builder.HasMany(p=>p.patrimonios).WithOne(p=>p.MobiliaTipoApartamentos);
     
        }
    }
}