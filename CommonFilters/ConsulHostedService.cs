using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CommonLibs
{
    public class ConsulHostedService : IHostedService, IDisposable
    {
        private readonly IConsulClient _consulClient;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ConsulServiceConfiguration _serviceConfiguration;
        private readonly string _registrationId;
        private readonly ILogger _logger;
        private Timer? _timer;

        public ConsulHostedService(IConsulClient consulClient, IHostApplicationLifetime hostApplicationLifetime,
            ConsulServiceConfiguration serviceConfiguration, ILogger<ConsulHostedService> logger)
        {
            _consulClient = consulClient;
            _hostApplicationLifetime = hostApplicationLifetime;
            _serviceConfiguration = serviceConfiguration;
            _registrationId = $"{_serviceConfiguration.ServiceName}-{Dns.GetHostName()}-{Guid.NewGuid()}";
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var registration = new AgentServiceRegistration
            {
                ID = _registrationId,
                Name = _serviceConfiguration.ServiceName,
                Address = _serviceConfiguration.Address,
                Port = _serviceConfiguration.Port,
                Check = new AgentServiceCheck
                {
                    TTL = TimeSpan.FromSeconds(30),
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
                }
            };
            try
            {
                await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
                _timer = new Timer(UpdateTTL!, null, TimeSpan.Zero, TimeSpan.FromSeconds(25));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            _hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                _consulClient.Agent.ServiceDeregister(_registrationId).Wait();
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _consulClient.Agent.ServiceDeregister(_registrationId, cancellationToken);
            _timer?.Change(Timeout.Infinite, 0);
        }

        private async void UpdateTTL(object state)
        {
            var ttlCheckId = $"service:{_registrationId}";
            try
            {
                await _consulClient.Agent.PassTTL(ttlCheckId, "Service is healthy");
                _logger.LogInformation("Service {ServiceId} TTL check updated successfully.", _registrationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
