using ExchangeData.Models;
using MassTransit;
using StatisticDbContext;

namespace QueueService.Consumers
{
    internal class FinishHuntConsumer : IConsumer<FinishHuntModel>
    {
        private readonly IStatisticContext _dbContext;
        private readonly ILogger<FinishHuntConsumer> _logger;

        public FinishHuntConsumer(IStatisticContext dbContext, ILogger<FinishHuntConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<FinishHuntModel> context)
        {
            await Task.Delay(0);
            CancellationToken cancellationToken = CancellationToken.None;
        }
    }
}
