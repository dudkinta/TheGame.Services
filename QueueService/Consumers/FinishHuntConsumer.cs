using ExchangeData;
using ExchangeData.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StatisticDbContext;
using System.Text;

namespace QueueService.Consumers
{
    public class FinishHuntConsumer : BackgroundService, IDisposable
    {
        private readonly ILogger<FinishHuntConsumer> _logger;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly IStatisticContext _context;
        private IConnection? _connection;
        private IModel? _channel;

        public FinishHuntConsumer(ILogger<FinishHuntConsumer> logger, RabbitMqSettings rabbitMqSettings, IStatisticContext context)
        {
            _logger = logger;
            _rabbitMqSettings = rabbitMqSettings;
            _context = context;
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

            _channel.QueueDeclare(queue: "ReferalQueue",
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"[ReferalConsumer] Received {message}");

                try
                {
                    var jObj = JsonConvert.DeserializeObject<FinishHuntModel>(message);
                    if (jObj == null)
                        throw new FormatException("FinishHuntModel not deserialized");

                    //var refOwner = await _context.Friends.FirstOrDefaultAsync(_ => _.id == jObj.Id);
                    //if (refOwner == null)
                    //{
                    //    var data = new FriendModel { id = jObj.Id, refer_id = jObj.Refer_id };
                    //    await _context.Friends.AddAsync(data, cancellationToken);
                    //    await _context.SaveAsync(cancellationToken);
                    //}
                    await Task.Delay(0);
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
