

using System.Text;
using Hotel.Application.Common;
using Hotel.Application.Contracts;
using Hotel.Application.Interfaces;
using Hotel.Domain.Identity;
using Hotel.Identity.Data;
using Hotel.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Hotel.Identity
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection
        services, IConfiguration configuration)
        {

            

            services.AddDbContextFactory<IdentityDataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("GHotelBbContext") ??
                    throw new InvalidOperationException("connection string 'BlogBbContext not found '"))
            );

   services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultTokenProviders();

          //  services.AddScoped<IIdentityService, IdentityService>();            
services.AddScoped<IAuthService, AuthService>();
services.AddLogging();
services.AddSingleton<ILoggerFactory, LoggerFactory>();
services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        /*     services.AddIdentity<ApplicationUser, IdentityRole>(
                 opt =>
            {
                opt.SignIn.RequireConfirmedEmail = true;
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequiredLength = 8;
            } 
            )
            .AddEntityFrameworkStores<IdentityDataContext>().AddDefaultTokenProviders();
 */

        //           services.AddTransient<IUserService, UserService>();

/*                                  services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                        })
                            .AddJwtBearer(o =>
                            {
                                var jwtSettings = services.BuildServiceProvider().GetRequiredService<JwtSettings>();

                                o.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuerSigningKey = true,
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    ClockSkew = TimeSpan.Zero,
                                    ValidIssuer =  jwtSettings.Issuer, // configuration["JwtSettings:Issuer"],
                                    ValidAudience = jwtSettings.Audience, //configuration["JwtSettings:Audience"],
                                    IssuerSigningKey =   new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                                };
                                o.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully.");
            return Task.CompletedTask;
        }
    };
                            });   */

            return services;
        }
    }
}