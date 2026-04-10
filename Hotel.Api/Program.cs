

using System.Text.Json.Serialization;
using Hotel.Application;
using Hotel.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using HOTEL;
using Serilog;
using Microsoft.OpenApi.Models;
using Hotel.Application.Middleware;
using Hotel.Application.Interfaces;
using Hotel.Infrastruture.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Hotel.Api.Extensions;



var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
//Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .WriteTo.File("log.txt",
        rollingInterval: RollingInterval.Minute,
        rollOnFileSizeLimit: true)
    .CreateLogger();

builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddInfrastrutureService(builder.Configuration);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);
//builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

//builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddMvc().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}
 );


builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    o.SerializerSettings.Converters.Add(new StringEnumConverter());
}).AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); //serialize all enums
});
builder.Services.AddCors(o =>
{
    o.AddPolicy("CorsPolicy", builder =>
    builder.AllowAnyOrigin()

        .AllowAnyMethod()
        .AllowAnyHeader());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelApi", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Host.UseSerilog(Log.Logger);
builder.Services.AddMvc() //x=> x.Filters.Add(new CustomExceptionFilter())
.AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
.AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    o.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
});



var app = builder.Build();
Log.Information("API Inicializado....");
app.Logger.LogInformation("calling from program");
app.UseSerilogRequestLogging();
//app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseRouting();
app.UseCors("CorsPolicy");

Log.Information("Registrando TenantResolutionMiddleware...");
app.UseMiddleware<TenantResolutionMiddleware>();
Log.Information("TenantResolutionMiddleware registrado.");
app.UseMiddleware<BaseCommandResponseMiddleware>();
app.UseMiddleware<AuditMiddleware>();
//app.UseMiddleware<GlobalErrorHandlingMiddleware>()

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (SecurityTokenExpiredException)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Token expirado");
        
    }
});

app.MapControllers();


app.Run();
 
/*  
 using System.Text.Json.Serialization;
using Hotel.Application;
using Hotel.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using HOTEL;
using Serilog;
using Microsoft.OpenApi.Models;
using Hotel.Application.Middleware;
using Hotel.Application.Interfaces;
using Hotel.Infrastruture.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Hotel.Infrastruture.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Minute, rollOnFileSizeLimit: true)
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

// Configuração de Serviços
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(o =>
{
    o.AddPolicy("CorsPolicy", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = "http://localhost",
            ValidAudience = "Audience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ChaveMuitoSecretaMuitoLonga1234567890!"))
        };
        o.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                if (context.AuthenticateFailure is SecurityTokenExpiredException)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\": \"Token expirado\"}");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelApi", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddInfrastrutureService(builder.Configuration);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityService(builder.Configuration);

builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddNewtonsoftJson(o =>
    {
        o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        o.SerializerSettings.Converters.Add(new StringEnumConverter());
    });

var app = builder.Build();

// Configuração do pipeline HTTP
Log.Information("API Inicializada com sucesso.");
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseMiddleware<BaseCommandResponseMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseMiddleware<AuditMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

// Mapear o SignalR Hub
app.MapHub<Hotel.Api.Hubs.RackHub>("/rackHub");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao iniciar.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
*/