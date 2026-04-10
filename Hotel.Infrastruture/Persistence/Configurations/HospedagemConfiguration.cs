using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class HospedagemConfiguration : IEntityTypeConfiguration<Hospedagem>
    {
        public void Configure(EntityTypeBuilder<Hospedagem> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(p=>p.Empresas).WithMany(p=>p.Hospedagens).HasForeignKey(m=>m.EmpresasId); 
            builder.HasOne(p=>p.MotivoViagens).WithMany(p=>p.Hospedagens).HasForeignKey(m=>m.MotivoViagensId);;   
            builder.HasOne(p=>p.TipoHospedagens).WithMany(p=>p.Hospedagens).HasForeignKey(m=>m.TipoHospedagensId);
           /*  builder.HasOne(p=>p.Apartamentos).WithMany(p=>p.Hospedagems)
                    .HasForeignKey(m=>m.ApartamentosId); */
                    builder.Property(m=>m.DataFechamento).IsRequired(false) ;
           

            builder.HasOne(p=>p.Checkins).WithMany(p=>p.Hospedagem)
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasForeignKey(m=>m.CheckinsId);
        }
    }
}