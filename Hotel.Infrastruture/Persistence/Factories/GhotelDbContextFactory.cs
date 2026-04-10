using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Infrastruture.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Hotel.Infrastruture.Persistence.Factories
{
    public class GhotelDbContextFactory : IDesignTimeDbContextFactory<GhotelDbContext>
    {
        private readonly IConfiguration _configuration;
        public GhotelDbContextFactory()
        {
            
        }

        public GhotelDbContextFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public GhotelDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GhotelDbContext>();
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("GHotelBbContext"));

        // Mock configuration
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:GHotelBbContext", "Data Source=SQL1004.site4now.net;Initial Catalog=db_abc40d_ghotel2025;User Id=db_abc40d_ghotel2025_admin;Password=RENT2024;Encrypt=false;" }
            })
            .Build();
            // Return with null tenant service since it's design-time
            return new GhotelDbContext(optionsBuilder.Options, null, configuration);
        }
    }
}