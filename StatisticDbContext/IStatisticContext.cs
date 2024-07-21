using Microsoft.EntityFrameworkCore;
using StatisticDbContext.Models;

namespace StatisticDbContext
{
    public interface IStatisticContext
    {
        DbSet<StorageModel> Storage { get; set; }
        Task<int> SaveAsync(CancellationToken cancellationToken);
    }
}
