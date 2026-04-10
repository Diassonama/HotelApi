using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
    {
        public void Configure(EntityTypeBuilder<Reserva> builder)
        {
            builder.HasKey(p => p.Id);
            
            // Configuração da propriedade decimal
            builder.Property(p => p.TotalGeral)
                   .HasColumnType("decimal(18,2)");
                   
          //  builder.HasOne(o=>o.Clientes).WithMany(o=>o.Reservas);
            builder.HasOne(o=>o.Utilizadores).WithMany(o=>o.Reservas);
            builder.HasOne(o=>o.Empresas).WithMany(o=>o.Reservas)
            .OnDelete(DeleteBehavior.NoAction);
          //  builder.HasOne(o=>o.TipoHospedagens).WithMany(o=>o.Reservas);



        /*     public Utilizador Utilizadores { get; set; }
        public Cliente Clientes { get; set; }
        public Empresa Empresas { get; set; }
        public Hospedagem Hospedagens { get; set; }
        public TipoPagamento TipoPagamentos { get; set; } */
        }
    }
}