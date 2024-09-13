using ExchangeData;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StatisticDbContext;
using StatisticDbContext.Models;
using System.Text;

namespace QueueService.Consumers
{
    public class CraftConsumer : BackgroundService
    {
        private readonly ILogger<CraftConsumer> _logger;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IConnection? _connection;
        private IModel? _channel;
        public CraftConsumer(ILogger<CraftConsumer> logger, RabbitMqSettings rabbitMqSettings, IServiceScopeFactory serviceScopeFactory)
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

            _channel.QueueDeclare(queue: "CraftQueue",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"[CraftQueueConsumer] Received {message}");

                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<IStatisticContext>();
                    var jObj = JsonConvert.DeserializeObject<CraftModel>(message);
                    if (jObj == null)
                        throw new FormatException("CraftModel not deserialized");

                    var craft = await context.Crafts.Include(_ => _.recipe).FirstOrDefaultAsync(_ => _.id == jObj.id);
                    if (craft == null)
                        throw new NullReferenceException("Craft not found");

                    if (craft.dt_end > DateTime.UtcNow)
                        throw new Exception("Craft not completed");

                    context.Crafts.Remove(craft);
                    await context.Inventory.AddAsync(new InventoryModel()
                    {
                        item_id = jObj.recipe.item_id,
                        user_id = jObj.user_id,
                    }, cancellationToken);

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

            _channel.BasicConsume(queue: "CraftQueue", autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
