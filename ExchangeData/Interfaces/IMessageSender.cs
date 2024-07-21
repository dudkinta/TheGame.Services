namespace ExchangeData.Interfaces
{
    public interface IMessageSender
    {
        Task SendMessage<T>(T message, string routingKey, CancellationToken cancellationToken) where T : class;
    }
}
