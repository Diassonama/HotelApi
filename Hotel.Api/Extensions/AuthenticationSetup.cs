using System.Text;
using Hotel.Identity.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
namespace Hotel.Api.Extensions
{
    public static class AuthenticationSetup
    {
        public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtSettings));
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JwtSettings:Key").Value));

            //         var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JwtSettings:Key").Value));
            //new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuaChaveSecreta"))
            services.Configure<JwtSettings>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtSettings.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtSettings.Audience)];
                options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
                options.AccessTokenExpiration = int.Parse(jwtAppSettingOptions[nameof(JwtSettings.AccessTokenExpiration)] ?? "0");
                options.RefreshTokenExpiration = int.Parse(jwtAppSettingOptions[nameof(JwtSettings.RefreshTokenExpiration)] ?? "0");
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration.GetSection("JwtSettings:Issuer").Value,

                ValidateAudience = true,
                ValidAudience = configuration.GetSection("JwtSettings:Audience").Value,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = tokenValidationParameters;
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = "invalid_token",
                error_description = "Token has expired or is invalid."
            };

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
    };
});






        }


        public static void AddAuthorizationPolicies(this IServiceCollection services)
        {
            //services.AddSingleton<IAuthorizationHandler, HorarioComercialHandler>();
            services.AddAuthorization();
        }
    }
}
