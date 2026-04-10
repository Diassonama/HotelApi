using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class UtilizadorConfiguration : IEntityTypeConfiguration<Utilizador>
    {
        public void Configure(EntityTypeBuilder<Utilizador> builder)
        {
            //    builder.HasKey(p => p.Id);
            //   builder.HasOne(o=>o.Perfis).WithMany(o=>o.Utilizadores).HasForeignKey(m=>m.PerfisId);
            /* builder.HasData(
                new Utilizador { Id = "1", Email="florydiasso@yahoo.com",EmailConfirmed= true,PhoneNumberConfirmed = true,TwoFactorEnabled= false,
                AccessFailedCount= 0,LockoutEnabled= false, PasswordHash ="123"  }
            );  */

           

        }
    }
}