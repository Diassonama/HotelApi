using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class MobiliaApartamentoConfiguration : IEntityTypeConfiguration<MobiliaApartamento>
    {
        public void Configure(EntityTypeBuilder<MobiliaApartamento> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p=>p.Patrimonios).WithMany(p=>p.MobiliaApartamentos);
      //     builder.HasOne(p=>p.TipoApartamentos).WithMany(p=>p.MobiliaApartamentos);
        }
    }
}