using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class CheckinConfiguration : IEntityTypeConfiguration<Checkins>
    {
        public void Configure(EntityTypeBuilder<Checkins> builder)
        {
            builder.HasKey(p => p.Id);
            builder.HasOne(c => c.CaixaCheckin)
               .WithMany()
               .HasForeignKey(c => c.IdCaixaCheckin)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.CaixaCheckout)
                .WithMany()
                .HasForeignKey(c => c.IdCaixaCheckOut)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.UtilizadoresCheckin).WithMany()
                       .HasForeignKey(c => c.IdUtilizadorCheckin)
                       .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.UtilizadoresCheckout).WithMany()
                       .HasForeignKey(c => c.IdUtilizadorCheckOut)
                       .OnDelete(DeleteBehavior.Restrict);

            builder.Property(m=>m.situacaoDoPagamento).HasConversion<string>();

             builder
            .HasOne(c => c.FacturaEmpresa)
            .WithOne(f => f.checkins)
            .HasForeignKey<FacturaEmpresa>(f => f.CheckinsId)
            .OnDelete(DeleteBehavior.Cascade); // Se excluir o check-in, remove a fatura associada (se houver)

            
        }
    }
}