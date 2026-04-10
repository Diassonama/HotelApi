using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class ApartamentoReservadoConfiguration : IEntityTypeConfiguration<ApartamentosReservado>
    {        public void Configure(EntityTypeBuilder<ApartamentosReservado> builder)
        {
            builder.HasKey(p => p.Id);

            // Configuração explícita da propriedade UtilizadoresId
            builder.Property(p => p.UtilizadoresId)
                   .HasColumnName("UtilizadoresId")
                   .HasColumnType("nvarchar(450)")
                   .IsRequired(false); // Tornar opcional

            // Configuração das propriedades decimais
            builder.Property(p => p.ValorDiaria)
                   .HasColumnType("decimal(18,2)");
            
            builder.Property(p => p.Total)
                   .HasColumnType("decimal(18,2)");

            builder.HasOne(p=>p.Apartamentos).WithMany(a => a.ApartamentosReservados)
                   .HasForeignKey(p => p.ApartamentosId)
                   .HasConstraintName("FK_ApartamentosReservados_Apartamentos_ApartamentosId")
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Reservas).WithMany(r => r.ApartamentosReservados)
                   .HasForeignKey(p => p.ReservasId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.Clientes).WithMany()
                   .HasForeignKey(p => p.ClientesId)
                   .HasConstraintName("FK_ApartamentosReservados_Clientes_ClientesId")
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.TipoHospedagens).WithMany()
                   .HasForeignKey(p => p.TipoHospedagensId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Configuração explícita da relação com Utilizador usando apenas UtilizadoresId
            builder.HasOne(p => p.Utilizadores).WithMany()
                   .HasForeignKey(p => p.UtilizadoresId)
                   .HasConstraintName("FK_ApartamentosReservados_Utilizadores_UtilizadoresId")
                   .OnDelete(DeleteBehavior.NoAction)
                   .IsRequired(false); // Tornar relação opcional
        }
    }
}