using Consul;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace CommonLibs
{
    public class ConsulHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ConsulServiceConfiguration _serviceConfiguration;
        private readonly string _registrationId;

        public ConsulHostedService(IConsulClient consulClient, IHostApplicationLifetime hostApplicationLifetime, ConsulServiceConfiguration serviceConfiguration)
        {
            _consulClient = consulClient;
            _hostApplicationLifetime = hostApplicationLifetime;
            _serviceConfiguration = serviceConfiguration;
            _registrationId = $"{Dns.GetHostName()}-{Guid.NewGuid()}";
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var registration = new AgentServiceRegistration
            {
                ID = _registrationId,
                Name = _serviceConfiguration.Name,
                Address = _serviceConfiguration.Address,
                Port = _serviceConfiguration.Port,
                Check = new AgentServiceCheck
                {
                    HTTP = $"http://{_serviceConfiguration.Address}:{_serviceConfiguration.Port}/{_serviceConfiguration.HealthEndpoint}",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5),
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
                }
            };

            await _consulClient.Agent.ServiceRegister(registration, cancellationToken);

            _hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                _consulClient.Agent.ServiceDeregister(_registrationId).Wait();
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _consulClient.Agent.ServiceDeregister(_registrationId, cancellationToken);
        }
    }
}
