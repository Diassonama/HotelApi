using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static System.Console;
using Microsoft.EntityFrameworkCore.Diagnostics;
//using Hotel.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Hotel.Domain.Identity;

namespace Hotel.Identity.Data;

public class IdentityDataContext: IdentityDbContext<ApplicationUser, IdentityRole,string>
{
    public IdentityDataContext()
    {
        
    }
   // public DbSet<ApplicationUser> applicationUsers { get; set; }
    public IdentityDataContext(DbContextOptions<IdentityDataContext> options) : base(options)
    {
    }
     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Data Source=SQL1004.site4now.net;Initial Catalog=db_abc40d_ghotel2025;User Id=sa;Password=RENT2024;Encrypt=false")
    .LogTo(WriteLine, new[] { RelationalEventId.CommandExecuted })
    .EnableSensitiveDataLogging(); 
 
}
