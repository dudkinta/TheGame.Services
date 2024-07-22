namespace ExchangeData.Interfaces
{
    public interface IMessageSender
    {
        Task SendMessage<T>(T message, RabbitRoutingKeys routingKey, CancellationToken cancellationToken) where T : class;
    }
}
