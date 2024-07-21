namespace ExchangeData
{
    public class RabbitMqSettings
    {
        public string Host { get; set; } = "rabbitmq://localhost";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string? Exchange { get; set; }
    }
}
