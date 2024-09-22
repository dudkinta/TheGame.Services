using ExchangeData;
using ExchangeData.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StatisticDbContext;
using System.Text;

namespace QueueService.Consumers
{
    public class FinishHuntConsumer : BackgroundService
    {
        private readonly ILogger<FinishHuntConsumer> _logger;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IConnection? _connection;
        private IModel? _channel;

        public FinishHuntConsumer(ILogger<FinishHuntConsumer> logger, RabbitMqSettings rabbitMqSettings, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _rabbitMqSettings = rabbitMqSettings;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqSettings.Host,
                UserName = _rabbitMqSettings.Username,
                Password = _rabbitMqSettings.Password
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "HuntingResult",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"[FinishHuntConsumer] Received {message}");

                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<IStatisticContext>();
                    var jObj = JsonConvert.DeserializeObject<FinishHuntModel>(message);
                    if (jObj == null)
                        throw new FormatException("HuntResultModel not deserialized");

                    _logger.LogInformation("Message: {@jObj}", jObj);

                    var storage = await context.Storage.FirstOrDefaultAsync(_ => _.user_id == jObj.Id);
                    if (storage != null)
                    {
                        storage.aim += jObj.AddAims;
                        storage.hunts++;
                        storage.shots += jObj.AddShots;
                        storage.main_coin += jObj.coins;
                    }
                    if (jObj.Items != null && jObj.Items.Count() > 0)
                    {
                        foreach (var item in jObj.Items)
                        {
                            await context.Inventory.AddAsync(new StatisticDbContext.Models.InventoryModel()
                            {
                                user_id = jObj.Id,
                                item_id = item.id
                            });
                        }
                    }
                    if (jObj.Heroes != null && jObj.Heroes.Count() > 0)
                    {
                        foreach (var hero in jObj.Heroes)
                        {
                            await context.Barracks.AddAsync(new StatisticDbContext.Models.BarrackModel()
                            {
                                user_id = jObj.Id,
                                hero_id = hero.id
                            });
                        }
                    }
                    await context.SaveAsync(cancellationToken);
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (FormatException ex)
                {
                    _logger.LogError(ex, ex.Message);
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: "HuntingResult", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
