using ExchangeData.Interfaces;
using MassTransit;

namespace ExchangeData.Helpers
{
    public class MessageSender : IMessageSender
    {
        private readonly IBus _bus;
        private readonly RabbitMqSettings _rabbitMqSettings;

        public MessageSender(IBus bus, RabbitMqSettings rabbitMqSettings)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _rabbitMqSettings = rabbitMqSettings ?? throw new ArgumentNullException(nameof(rabbitMqSettings));
        }

        public async Task SendMessage<T>(T message, RabbitRoutingKeys routingKey, CancellationToken cancellationToken) where T : class
        {
            var sendEndpoint = await _bus.GetSendEndpoint(new Uri($"exchange:{_rabbitMqSettings.Exchange}"));
            await sendEndpoint.Send(message, context =>
            {
                context.SetRoutingKey(routingKey.ToString());
                context.Durable = true;
            }, cancellationToken);
        }
    }
}
