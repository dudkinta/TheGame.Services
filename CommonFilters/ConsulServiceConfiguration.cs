namespace CommonLibs
{
    public class ConsulServiceConfiguration
    {
        public string Endpoint { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Port { get; set; }
        public string HealthEndpoint { get; set; } = string.Empty;
    }
}
