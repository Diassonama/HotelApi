using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class HistoricoConfiguration : IEntityTypeConfiguration<Historico>
    {
        public void Configure(EntityTypeBuilder<Historico> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p=>p.Utilizadores).WithMany(p=>p.Historicos).HasForeignKey(m=>m.UtilizadoresId);
            builder.HasOne(p=>p.Checkins).WithMany(p=>p.Historicos).HasForeignKey(m=>m.CheckinsId);
            builder.HasOne(p=>p.Hospedagem).WithMany(p=>p.Historicos);
        }
    }
}