using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class MenuRoleConfiguration : IEntityTypeConfiguration<MenuRole>
    {
        public void Configure(EntityTypeBuilder<MenuRole> builder)
        {
            builder.HasKey(e => new { e.MenuId, e.RoleId });
            //  builder.HasOne(m=>m.Menus).WithMany(m=>m.MenuRole);
            //  builder.HasOne(m=>m.Roles).WithMany();
            builder.HasOne(e => e.Menu)
                .WithMany(m => m.MenuRole)
                .HasForeignKey(e => e.MenuId);
                
            builder.HasOne(e => e.Roles)
                  .WithMany()
                  .HasForeignKey(e => e.RoleId);
        }
    }
}