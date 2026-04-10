using Hotel.Application.Common;
using Hotel.Application.Contracts;
using Hotel.Application.Interfaces;
using Hotel.Application.Interfaces.Services;
using Hotel.Domain.Identity;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Hotel.Domain.Tenant.Entities;
using Hotel.Infrastruture.Identity;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Repositories;
using Hotel.Infrastruture.Persistence.Shared;
using Hotel.Infrastruture.Services;
using System;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HOTEL;

public static class ConfigureServices
{

    public static IServiceCollection AddInfrastrutureService(this IServiceCollection
        services, IConfiguration configuration)
    {
         services.AddHttpContextAccessor();
       /*  services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

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
            }); */



        services.AddDbContextFactory<TenantContext>(options =>
           options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ??
               throw new InvalidOperationException("connection string 'TenantContext not found '")));

       





        services.AddDbContext<GhotelDbContext>((serviceProvider, options) =>
         {


             var tenantService = serviceProvider.GetRequiredService<ITenantService>();
             var tenant = tenantService.GetCurrentTenant();

             //     var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

             // Recupera o Tenant do contexto HTTP (setado pela middleware)
             // var tenant = httpContextAccessor.HttpContext?.Items["TenantId"] as Tenant;
             /*  var tenantId = context.Request.Headers["TenantId"].FirstOrDefault()
                                     ?? context.Request.Host.Host.Split('.')[0];
          */
             if (tenant != null && !string.IsNullOrEmpty(tenant.ConnectionString))
             {
                 options.UseSqlServer(tenant.ConnectionString);
             }
             else
             {
                 options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                 // Configura uma conexão padrão ou lança uma exceção
                 //throw new InvalidOperationException("A connection string is required to configure the database context.");
             }
         });



        services.AddScoped<DbContext, TenantContext>();

        services.AddIdentityCore<ApplicationUser>(opt =>
                   {
                       opt.SignIn.RequireConfirmedEmail = true;
                       opt.Password.RequireDigit = true;
                       opt.Password.RequireLowercase = true;
                       opt.Password.RequireUppercase = true;
                       opt.Password.RequireNonAlphanumeric = true;
                       opt.Password.RequiredLength = 6;
                   })
                   .AddRoles<IdentityRole>() // Include roles if your application uses them
       .AddSignInManager<SignInManager<ApplicationUser>>() // Add SignInManager for sign-in functionality
       .AddDefaultTokenProviders()
       .AddEntityFrameworkStores<GhotelDbContext>();

       
        services.AddScoped<DbContext, GhotelDbContext>();
        services.AddScoped<SerialService>();


        #region "Scoped"
        services.AddTransient<Tenant>();
        //  services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<TenantCacheService>();
        services.AddScoped<IMenuRoleRepository, MenuRoleRepository>();
        //   services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ISerialService, SerialService>();
        services.AddMemoryCache();
        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
        services.AddScoped(typeof(IAcessoRepository), typeof(AcessoRepository));
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IRoleRepository, RoleRepository>();
        services.AddScoped<UsuarioLogado>();
        services.AddScoped<IEmailServerConfiguration, EmailServerConfiguration>();
        services.AddScoped<ICaixa, CaixaService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IConfigService, ConfigService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<ISMSService, SMSService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IClaimService, ClaimService>();
        services.AddScoped<IApartamentoRepository, ApartamentoRepository>();
        services.AddScoped<ISerialRepository, SerialRepository>();
        services.AddScoped<ISeriesRepository, SeriesRepository>();
         services.AddScoped<ITipoReciboRepository, TipoReciboRepository>();
          services.AddScoped<ITaxAccountingRepository, TaxAccountingBasisRepository>();

           services.AddScoped<ITaxTableEntryRepository, TaxTableEntryRepository>();
            services.AddScoped<ITaxExemptionReasonRepository, TaxExemptionReasonRepository>();

        services.AddScoped(typeof(IApartamentoReservadoRepository), typeof(ApartamentosReservadoRepository));
        services.AddScoped(typeof(ICaixaRepository), typeof(CaixaRepository));
        services.AddScoped(typeof(ICheckinRepository), typeof(CheckinRepository));
        services.AddScoped(typeof(IClienteRepository), typeof(ClienteRepository));
        services.AddScoped(typeof(IContaRepository), typeof(ContaRepository));
        services.AddScoped(typeof(IDespertadorRepository), typeof(DespertadorRepository));
        services.AddScoped(typeof(IEmpresaRepository), typeof(EmpresaRepository));
        services.AddScoped(typeof(IFacturaDivididaRepository), typeof(FacturaDivididaRepository));
        services.AddScoped(typeof(IFacturaEmpresaRepository), typeof(FacturaEmpresaRepository));
        services.AddScoped(typeof(IGovernancaRepository), typeof(GovernancaRepository));
        services.AddScoped(typeof(IHistoricoRepository), typeof(HistoricoRepository));
        services.AddScoped(typeof(IHospedagemRepository), typeof(HospedagemRepository));
        services.AddScoped(typeof(IHospedeRepository), typeof(HospedeRepository));
        services.AddScoped(typeof(ILancamentoCaixaRepository), typeof(LancamentoCaixaRepository));
        services.AddScoped(typeof(ILavandariaRepository), typeof(LavandariaRepository));
        services.AddScoped(typeof(ILavandariaItemRepository), typeof(LavandariaItemRepository));
        services.AddScoped(typeof(IMobiliaApartamentoRepository), typeof(MobiliaApartamentoRepository));
        services.AddScoped(typeof(IMotivoViagemRepository), typeof(MotivoViagemRepository));
        services.AddScoped(typeof(IPagamentoRepository), typeof(PagamentoRepository));
        services.AddScoped(typeof(IPaisRepository), typeof(PaisRepository));
        services.AddScoped(typeof(IParamRepository), typeof(ParamRepository));
        services.AddScoped(typeof(IPatrimonioRepository), typeof(PatrimonioRepository));
        services.AddScoped(typeof(IPerfilRepository), typeof(PerfilRepository));
        services.AddScoped(typeof(IPlanoDeContaRepository), typeof(PlanoDeContaRepository));
        services.AddScoped(typeof(IPontoDeVendarepository), typeof(PontoDeVendaRepository));
        services.AddScoped(typeof(IProductTypeRepository), typeof(ProductTypeRepository));
        services.AddScoped(typeof(IProdutoRepository), typeof(ProdutoRepository));
        services.AddScoped(typeof(IProdutoStockRepository), typeof(ProdutoStockRepository));
        services.AddScoped(typeof(IReservaRepository), typeof(ReservaRepository));
        services.AddScoped(typeof(ITaxRepository), typeof(TaxRepository));
        services.AddScoped(typeof(ITipoGovernancaRepository), typeof(TipoGovernancaRepository));
        services.AddScoped(typeof(ITipoHospedagemRepository), typeof(TipoHospedagemRepository));
        services.AddScoped(typeof(ITipoPagamentoRepository), typeof(TipoPagamentoRepository));
        services.AddScoped(typeof(ITipoProdutoRepository), typeof(TipoProdutoRepository));
        services.AddScoped(typeof(IUtilizadorRepository), typeof(UtilizadorRepository));
       
        services.AddScoped(typeof(IMotivoTransferenciaRepository), typeof(MotivoTransferenciaRepository));
        services.AddScoped(typeof(ITransferenciaQuartoRepository), typeof(TransferenciaQuartoRepository));
        services.AddScoped(typeof(ITransferenciaRepository), typeof(TransferenciaRepository));

        services.AddScoped(typeof(IMessageRepository), typeof(MessageRepository));
        services.AddScoped(typeof(IConversationRepository), typeof(ConversationRepository));
        services.AddScoped(typeof(IMessageNotificationRepository), typeof(MessageNotificationRepository));
        services.AddScoped(typeof(IOnlineStatusRepository), typeof(OnlineStatusRepository));
        services.AddScoped(typeof(IConversationParticipantRepository), typeof(ConversationParticipantRepository));
        // Services
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IRackNotificationService, RackNotificationService>();
        services.AddScoped<IReciboService, ReciboService>();
        services.AddScoped<ITransferenciaRepository , TransferenciaRepository>();
        services.AddScoped<IEmpresaSaldoRepository, EmpresaSaldoRepository>();
        services.AddScoped<IRelatorioSaldoService, RelatorioSaldoService>();
        services.AddScoped<IContaPagarRepository, ContaPagarRepository>();
        services.AddScoped<IContaReceberRepository, ContaReceberRepository>();
        services.AddScoped<IRelatorioContasService, RelatorioContasService>();
        #endregion
        return services;
    }
}