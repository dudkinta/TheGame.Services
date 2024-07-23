using ExchangeData;
using FriendDbContex;
using Microsoft.EntityFrameworkCore;
using QueueService;
using QueueService.Consumers;
using StatisticDbContext;


var builder = Host.CreateApplicationBuilder(args);

AddDbContext(builder.Services, builder.Configuration);
AddConsumers(builder.Services, builder.Configuration);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();


void AddDbContext(IServiceCollection services, IConfiguration configuration)
{
    var friendConnectionString = configuration.GetConnectionString("FriendConnection") ?? string.Empty;
    services.AddDbContext<IFriendContext, FriendContext>(options =>
        options.UseNpgsql(friendConnectionString));

    var statisticConnectionString = configuration.GetConnectionString("StatisticConnection") ?? string.Empty;
    services.AddDbContext<IStatisticContext, StatisticContext>(options =>
        options.UseNpgsql(friendConnectionString));
}

void AddConsumers(IServiceCollection services, IConfiguration configuration)
{
    var rabbitMqSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>() ?? new RabbitMqSettings();
    services.AddSingleton(rabbitMqSettings);

    services.AddHostedService<ReferalConsumer>();
    services.AddHostedService<FinishHuntConsumer>();
}