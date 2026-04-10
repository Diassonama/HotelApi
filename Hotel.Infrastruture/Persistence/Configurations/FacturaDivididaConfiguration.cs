using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class FacturaDivididaConfiguration : IEntityTypeConfiguration<FacturaDividida>
    {
        public void Configure(EntityTypeBuilder<FacturaDividida> builder)
        {
           builder.HasKey(p => p.Id);
           builder.HasOne(p=>p.Checkins).WithMany(p=>p.FacturaDivididas);
           builder.HasOne(p=>p.Hospedagens).WithMany(p=>p.FacturaDivididas);
        }
    }
}