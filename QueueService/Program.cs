using ExchangeData;
using FriendDbContex;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using QueueService;
using QueueService.Consumers;
using RabbitMQ.Client;


var builder = Host.CreateApplicationBuilder(args);

var friendConnectionString = builder.Configuration.GetConnectionString("FriendConnection") ?? string.Empty;
builder.Services.AddDbContext<IFriendContext, FriendContext>(options =>
    options.UseNpgsql(friendConnectionString));

var rabbitMqSettings = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMqSettings>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReferalConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqSettings!.Host, h =>
        {
            h.Username(rabbitMqSettings.Username);
            h.Password(rabbitMqSettings.Password);
        });
        cfg.ReceiveEndpoint("ReferalQueue", ep =>
        {
            ep.ConfigureConsumer<ReferalConsumer>(context);
            ep.Bind("LoginService", x =>
            {
                x.RoutingKey = "referal";
                x.ExchangeType = ExchangeType.Fanout;
            });
        });
    });
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();