
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using MediatR;
using Hotel.Application.Common.Behaviours;
using Microsoft.Extensions.Configuration;
using Hotel.Application.Common.Mapping;
using Hotel.Application.Common.Mappings;
using Hotel.Application.Services;
using Hotel.Application.Extensions;
using Hotel.Application.Contracts;
using Hotel.Application.Interfaces;
using Hotel.Application.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hotel.Application.TipoApartamento.Commands;

namespace Hotel.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection
        services, IConfiguration configuration)
        {
 services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        services.AddSingleton(sp =>
           {
               var jwtSettings = sp.GetRequiredService<IOptions<JwtSettings>>().Value;
               if (string.IsNullOrEmpty(jwtSettings.Key))
               {
                   throw new InvalidOperationException("A chave de assinatura não está configurada no JwtSettings.");
               }
               jwtSettings.SigningCredentials = new SigningCredentials(
                   new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                   SecurityAlgorithms.HmacSha256
               );

               return jwtSettings;
           });
           
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
			services.AddTransient(typeof(IPipelineBehavior<,>),typeof(ValidationBehaviour<,>));
          //  services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
           // 	services.AddScoped<IMapFrom, MappingProfile>();
      
           services.AddAutoMapper(typeof(MappingProfile));
           services.AddScoped<UsuarioLogado>();
           services.AddScoped<IpAddressService>();

         //  services.AddSignalR();
          

            services.AddMediatR(x =>   {
            x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            x.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            });

//services.AddScoped<IValidator<UpdateTipoApartamentoCommand>, UpdateTipoApartamentoCommandValidator>();

/*             services.AddMvc()
              .AddJsonOptions(opt => {
                  opt.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()); //serialize all enums
              }); */
          
			return services;
        }
    }
}