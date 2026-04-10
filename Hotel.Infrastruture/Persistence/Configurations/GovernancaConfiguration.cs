using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class GovernancaConfiguration : IEntityTypeConfiguration<Governanca>
    {
        public void Configure(EntityTypeBuilder<Governanca> builder)
        {
                builder.HasKey(p => p.Id);
                builder.HasOne(p=>p.TipoGovernancas).WithMany(p=>p.Governancas)
                            .HasForeignKey(m=>m.TipoGovernancasId);
                builder.HasOne(p=>p.Apartamentos).WithMany(p=>p.Governancas)
                            .HasForeignKey(m=>m.TipoGovernancasId);

              //  builder.HasOne(p=>p.TipoApartamentos).WithMany(p=>p.Governancas);
        }
    }
}