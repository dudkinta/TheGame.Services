using Consul;

namespace CommonLibs
{
    public class ConsulServiceDiscovery
    {
        private readonly IConsulClient _consulClient;

        public ConsulServiceDiscovery(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        public async Task<string?> GetServiceAddress(string serviceName)
        {
            var services = await _consulClient.Agent.Services();
            var service = services.Response.Values.FirstOrDefault(s => s.Service.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            return service != null ? $"{service.Address}:{service.Port}" : null;
        }
    }
}
