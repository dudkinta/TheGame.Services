using Microsoft.EntityFrameworkCore;
using StatisticDbContext.Models;

namespace StatisticDbContext
{
    public interface IStatisticContext
    {
        DbSet<StorageModel> Storage { get; set; }
        DbSet<ItemModel> Items { get; set; }
        DbSet<HeroModel> Heroes { get; set; }
        DbSet<InventoryModel> Inventory { get; set; }
        DbSet<BarrackModel> Barracks { get; set; }
        DbSet<ArmyModel> Armies { get; set; }
        DbSet<RecipeModel> Recipes { get; set; }
        DbSet<RecipeIngredientModel> RecipeIngredients { get; set; }
        Task<int> SaveAsync(CancellationToken cancellationToken);
    }
}
