using ExchangeData;
using FriendDbContex;
using Microsoft.EntityFrameworkCore;
using QueueService;
using QueueService.Consumers;
using Serilog;
using Serilog.Formatting.Compact;
using StatisticDbContext;


var builder = Host.CreateApplicationBuilder(args);
AddLogger(builder.Logging);
AddDbContext(builder.Services, builder.Configuration);
AddConsumers(builder.Services, builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();

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

void AddDbContext(IServiceCollection services, IConfiguration configuration)
{
    var friendConnectionString = configuration.GetConnectionString("FriendConnection") ?? string.Empty;
    services.AddDbContext<IFriendContext, FriendContext>(options =>
        options.UseNpgsql(friendConnectionString));

    var statisticConnectionString = configuration.GetConnectionString("StatisticConnection") ?? string.Empty;
    services.AddDbContext<IStatisticContext, StatisticContext>(options =>
        options.UseNpgsql(statisticConnectionString));
}

void AddConsumers(IServiceCollection services, IConfiguration configuration)
{
    var rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>() ?? throw new ArgumentNullException(nameof(RabbitMqSettings));
    services.AddSingleton(rabbitMqSettings!);

    services.AddSingleton<IHostedService, ReferalConsumer>();
    services.AddSingleton<IHostedService, FinishHuntConsumer>();
    services.AddSingleton<IHostedService, CraftConsumer>();
}