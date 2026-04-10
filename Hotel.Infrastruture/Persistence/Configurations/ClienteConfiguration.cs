using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.HasKey(p => p.Id);
         //   builder.Property(p=>p.Id).UseIdentityColumn();
            builder.Property(p => p.DataAniversario).HasColumnType("Date");
                
            builder.HasOne(p=>p.Paises)
                .WithMany(p=>p.Clientes).HasForeignKey(p=>p.PaisId);

            builder.HasOne(p=>p.Empresa).WithMany(p=>p.Clientes).HasForeignKey(c=>c.EmpresasId);    

            //builder.Property(p => p.Generos).HasConversion(
            //    p => p.ToString(),
            //    p => (Genero)Enum.Parse(typeof(Genero), p));
           // builder.Property(c => c.Generos);
            builder.Property(c => c.Generos)
                .HasConversion<string>();

            // Configuração explícita da relação com ApartamentosReservados para evitar propriedade ClienteId automática
            builder.HasMany(c => c.ApartamentosReservados)
                   .WithOne(ar => ar.Clientes)
                   .HasForeignKey(ar => ar.ClientesId)
                   .OnDelete(DeleteBehavior.Restrict);
                   
                     builder.HasMany(c => c.Hospedes)
                   .WithOne(h => h.Clientes)
                   .HasForeignKey(h => h.ClientesId)
                   .OnDelete(DeleteBehavior.Restrict);

            // builder.Property(c=> c)

            //builder.HasOne(c => c.Generos).WithMany().HasForeignKey(c => c.Generos); ;

            builder.HasData(
               // new Cliente { Id = 1, Nome = "Cliente Diversos",  DataAniversario = new DateTime(1990, 5, 15), Generos = Domain.Enums.Genero.Masculino, PaisId = 1,IdEmpresa=1,PercetualDesconto=0,DateCreated=DateTime.Now,LastModifiedDate= DateTime.Now, IsActive= true }
        //new Cliente { Id = 1, Nome = "Cliente Diversos", Email ="florydiasso@yahoo.com"}

            );   
        }
    }
}