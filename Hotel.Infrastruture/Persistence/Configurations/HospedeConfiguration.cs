using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class HospedeConfiguration : IEntityTypeConfiguration<Hospede>
    {
        public void Configure(EntityTypeBuilder<Hospede> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p=>p.Clientes).WithMany(p=>p.Hospedes).HasForeignKey(m=>m.ClientesId);
            builder.HasOne(p=>p.checkins).WithMany(p=>p.Hospedes).HasForeignKey(p=>p.CheckinsId);
            builder.Property(m=>m.Estado).HasConversion<string>();
           // builder.HasMany(p=>p.Pedidos).WithOne(p=>p.Hospedes);
        }
    }
}