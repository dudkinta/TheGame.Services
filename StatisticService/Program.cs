using CommonLibs;
using Consul;
using ExchangeData;
using ExchangeData.Helpers;
using ExchangeData.Interfaces;
using InnerApiLib;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StatisticDbContext;

var builder = WebApplication.CreateBuilder(args);

AddFilters(builder.Services);
AddServices(builder.Services, builder.Configuration);
RegistrationConsul(builder.Services, builder.Configuration);
AddAuthorize(builder.Services);


var app = builder.Build();

app.UsePathBase(new PathString("/api/statistic"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


void AddFilters(IServiceCollection services)
{
    services.AddControllers(options =>
    {
        options.Filters.Add<BadRequestFilter>();
    });
}

void AddServices(IServiceCollection services, IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
    services.AddDbContext<IStatisticContext, StatisticContext>(options =>
    options.UseNpgsql(connectionString));

    var rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();
    services.AddSingleton(rabbitMqSettings!);

    services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(rabbitMqSettings!.Host, h =>
            {
                h.Username(rabbitMqSettings.Username);
                h.Password(rabbitMqSettings.Password);
            });
        });
    });
    services.AddScoped<IMessageSender, MessageSender>();
    services.AddScoped<IInnerApiClient, InnerApiClient>();
}

void AddAuthorize(IServiceCollection services)
{
    services.AddSingleton<JwtSecretProvider>();

    services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsFactory>();

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer("User", options => { })
    .AddJwtBearer("Service", options => { });

    services.AddAuthorization(options =>
    {
        options.AddPolicy("UserPolicy", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "user"));
        options.AddPolicy("ServicePolicy", policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "service"));
    });
}

void RegistrationConsul(IServiceCollection services, IConfiguration configuration)
{
    var consulConfigSection = configuration.GetSection("AppSettings:Consul");
    var consulServiceConfig = new ConsulServiceConfiguration
    {
        Name = consulConfigSection.GetValue<string>("ServiceName")!,
        Address = consulConfigSection.GetValue<string>("Address")!,
        Port = consulConfigSection.GetValue<int>("Port")!,
    };

    services.AddSingleton(consulServiceConfig);

    //Consul client
    var consulEndpoint = configuration.GetSection("AppSettings:ConsulEndpoint").Value ?? string.Empty;
    services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(cfg =>
    {
        cfg.Address = new Uri(consulEndpoint);
    }));
    services.AddHostedService<ConsulHostedService>();
    services.AddTransient<ConsulServiceDiscovery>();
}

