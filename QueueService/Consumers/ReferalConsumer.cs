using ExchangeData;
using ExchangeData.Models;
using FriendDbContex;
using FriendDbContex.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace QueueService.Consumers
{
    public class ReferalConsumer : BackgroundService, IDisposable
    {
        private readonly ILogger<ReferalConsumer> _logger;
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly IFriendContext _context;
        private IConnection? _connection;
        private IModel? _channel;

        public ReferalConsumer(ILogger<ReferalConsumer> logger, RabbitMqSettings rabbitMqSettings, IFriendContext context)
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
                    var jObj = JsonConvert.DeserializeObject<ReferModel>(message);
                    if (jObj == null)
                        throw new FormatException("ReferModel not deserialized");

                    var refOwner = await _context.Friends.FirstOrDefaultAsync(_ => _.id == jObj.Id);
                    if (refOwner == null)
                    {
                        var data = new FriendModel { id = jObj.Id, refer_id = jObj.Refer_id };
                        await _context.Friends.AddAsync(data, cancellationToken);
                        await _context.SaveAsync(cancellationToken);
                    }

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

            _channel.BasicConsume(queue: "ReferalQueue", autoAck: false, consumer: consumer);

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