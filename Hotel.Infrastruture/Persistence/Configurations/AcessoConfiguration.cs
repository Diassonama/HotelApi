using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class AcessoConfiguration : IEntityTypeConfiguration<Acesso>
    {
        public void Configure(EntityTypeBuilder<Acesso> builder)
        {
           builder.HasKey(p=> p.Id);
           builder.HasOne(p => p.Perfils).WithMany(p=> p.Acessos);
        }
    }
}