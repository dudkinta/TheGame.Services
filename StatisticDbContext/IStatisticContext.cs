using Microsoft.EntityFrameworkCore;
using StatisticDbContext.Models;

namespace StatisticDbContext
{
    public interface IStatisticContext
    {
        DbSet<StorageModel> Storage { get; set; }
        DbSet<InventoryModel> Inventory { get; set; }
        DbSet<ItemModel> Items { get; set; }
        Task<int> SaveAsync(CancellationToken cancellationToken);
    }
}
