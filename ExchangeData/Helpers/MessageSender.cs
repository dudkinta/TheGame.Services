using ExchangeData.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace ExchangeData.Helpers
{
    public class MessageSender : IMessageSender
    {
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageSender(RabbitMqSettings rabbitMqSettings)
        {
            _rabbitMqSettings = rabbitMqSettings ?? throw new ArgumentNullException(nameof(rabbitMqSettings));

            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqSettings.Host,
                UserName = _rabbitMqSettings.Username,
                Password = _rabbitMqSettings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: _rabbitMqSettings.Exchange, type: ExchangeType.Direct, durable: true, autoDelete: false);
        }

        public async Task SendMessage<T>(T message, RabbitRoutingKeys routingKey, CancellationToken cancellationToken) where T : class
        {
            var messageBody = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageBody);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            await Task.Run(() =>
            {
                _channel.BasicPublish(
                    exchange: _rabbitMqSettings.Exchange,
                    routingKey: routingKey.ToString(),
                    basicProperties: properties,
                    body: body);
            }, cancellationToken);
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
