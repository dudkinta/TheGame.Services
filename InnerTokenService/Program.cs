using CommonLibs;
using Consul;
using InnerTokenService.Helpers;
using InnerTokenService.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

AddFilters(builder.Services);
AddServices(builder.Services, builder.Configuration);
RegistrationConsul(builder.Services, builder.Configuration);
AddAuthorize(builder.Services);

var app = builder.Build();
app.UsePathBase(new PathString("/api/inner"));

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
    services.AddSingleton<JwtSecretProvider>();
    services.AddScoped<ITokenService, JwtTokenService>(provider =>
    {
        var jwtSecretProvider = provider.GetRequiredService<JwtSecretProvider>();
        var privateUserKey = jwtSecretProvider.GetSecretKey("jwt/service");
        return new JwtTokenService(privateUserKey);
    });
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
        HealthEndpoint = consulConfigSection.GetValue<string>("HealthEndpoint")!
    };

    services.AddSingleton(consulServiceConfig);

    //Consul client
    var consulEndpoint = configuration.GetSection("AppSettings:ConsulEndpoint").Value ?? string.Empty;
    services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(cfg =>
    {
        cfg.Address = new Uri(consulEndpoint);
    }));
    services.AddHostedService<ConsulHostedService>();

}