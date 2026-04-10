using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class EmpresaSaldoConfiguration: IEntityTypeConfiguration<EmpresaSaldo>
    {
        public void Configure(EntityTypeBuilder<EmpresaSaldo> builder)
        {
            builder.HasKey(e => e.Id);
            //builder.Property(e => e.Id).UseIdentityColumn();

            builder.HasOne(e => e.Empresa)
                   .WithMany(emp => emp.EmpresaSaldos)
                   .HasForeignKey(e => e.EmpresaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.EmpresaSaldoMovimentos)
                   .WithOne(ems => ems.EmpresaSaldo)
                   .HasForeignKey(ems => ems.EmpresaSaldoId)
                   .OnDelete(DeleteBehavior.Restrict);


                   

            builder.HasIndex(e => e.EmpresaId)
                   .IsUnique();
        }
    }
    
}