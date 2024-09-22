using CommonLibs;
using Consul;
using FriendDbContex;
using InnerApiLib;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);
AddLogger(builder.Logging);
AddFilters(builder.Services);
AddServices(builder.Services, builder.Configuration);
RegistrationConsul(builder.Services, builder.Configuration);
AddAuthorize(builder.Services);


var app = builder.Build();

app.UsePathBase(new PathString("/api/friends"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

void AddLogger(ILoggingBuilder logging)
{
    Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Http("http://localhost:5044",
                 queueLimitBytes: null,
                 textFormatter: new CompactJsonFormatter())
    .CreateLogger();

    logging.ClearProviders();
    logging.AddSerilog(Log.Logger);
}

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
    services.AddDbContext<IFriendContext, FriendContext>(options =>
        options.UseNpgsql(connectionString));

    services.AddScoped<IUserService, UserService>();
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
    var consulServiceConfig = configuration.GetSection("AppSettings:Consul").Get<ConsulServiceConfiguration>();
    if (consulServiceConfig != null)
    {
        services.AddSingleton(consulServiceConfig);

        //Consul client
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(cfg =>
        {
            cfg.Address = new Uri(consulServiceConfig.Endpoint);
            cfg.Token = consulServiceConfig.Token;
        }));
        services.AddHostedService<ConsulHostedService>();
        services.AddTransient<ConsulServiceDiscovery>();
    }
}
