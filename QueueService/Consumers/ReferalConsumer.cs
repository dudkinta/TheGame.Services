using ExchangeData.Models;
using FriendDbContex;
using FriendDbContex.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace QueueService.Consumers
{
    internal class ReferalConsumer : IConsumer<ReferModel>
    {
        private readonly IFriendContext _dbContext;
        private readonly ILogger<ReferalConsumer> _logger;

        public ReferalConsumer(IFriendContext dbContext, ILogger<ReferalConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ReferModel> context)
        {
            var refOwner = await _dbContext.Friends.FirstOrDefaultAsync(_ => _.id == context.Message.Id);
            if (refOwner == null)
            {
                CancellationToken cancellationToken = CancellationToken.None;
                var data = new FriendModel { id = context.Message.Id, refer_id = context.Message.Refer_id };
                await _dbContext.Friends.AddAsync(data, cancellationToken);
                await _dbContext.SaveAsync(cancellationToken);
            }
        }
    }
}
