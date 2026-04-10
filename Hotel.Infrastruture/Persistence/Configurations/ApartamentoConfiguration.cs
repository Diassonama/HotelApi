using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class ApartamentoConfiguration : IEntityTypeConfiguration<Apartamentos>
    {
  
        public void Configure(EntityTypeBuilder<Apartamentos> builder)
        {

            builder.HasKey(p => p.Id);
            builder.Property(p=>p.Id).UseIdentityColumn();
            builder.Property(p=>p.Codigo).HasMaxLength(15)
                    .IsRequired();
            builder.Property(o=>o.Observacao).HasMaxLength(200);
    //        builder.Property(p=>p.Situacao).HasMaxLength(1);

            builder.HasOne(p=> p.TipoApartamentos).WithMany(p=>p.Apartamentos)
                   .OnDelete(DeleteBehavior.NoAction)
                   .IsRequired().HasForeignKey(m=> m.TipoApartamentosId); 
            
            builder.Property(p=>p.Frigobar).HasMaxLength(1);

            builder.HasOne(x=>x.TipoGovernancas).WithMany(p=>p.Apartamentos)
                   .OnDelete(DeleteBehavior.NoAction)
                   .IsRequired().HasForeignKey(m=>m.TipoGovernancasId);  

            builder.HasOne(c=>c.checkins).WithMany(m=>m.apartamentos)
            .HasForeignKey(m=>m.CheckinsId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Configuração explícita da relação com ApartamentosReservados
            builder.HasMany(p => p.ApartamentosReservados)
                   .WithOne(ar => ar.Apartamentos)
                   .HasForeignKey(ar => ar.ApartamentosId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Configuração simplificada e robusta para enum Situacao
            builder.Property(c => c.Situacao)
                .HasConversion<string>() // Conversão automática do EF Core
                .HasColumnType("varchar(20)")
                .HasDefaultValue(Hotel.Domain.Enums.Situacao.Livre)
                .IsRequired();

            builder.Property(p => p.Frigobar).HasMaxLength(1);


builder.HasData(
    new Apartamentos(1,"Quarto-001",1 ),
    new Apartamentos(2,"Quarto-002",1 ),
    new Apartamentos(3,"Quarto-003",1 ),
    new Apartamentos(4,"Quarto-004",1 ),
    new Apartamentos(5,"Quarto-005",1 ),
    new Apartamentos(6,"Quarto-006",1 ),
    new Apartamentos(7,"Quarto-007",1 ),
    new Apartamentos(8,"Quarto-008",1 ),
    new Apartamentos(9,"Quarto-009",1 ),
    new Apartamentos(10,"Quarto-010",1 ),
    new Apartamentos(11,"Quarto-011",1 ),
    new Apartamentos(12,"Quarto-012",1 ),
    new Apartamentos(13,"Quarto-013",1 ),
    new Apartamentos(14,"Quarto-014",1 ),
    new Apartamentos(15,"Quarto-015",1 ),
    new Apartamentos(16,"Quarto-016",1 )
);
            
        }
    }
}