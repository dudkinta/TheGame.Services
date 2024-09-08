using Microsoft.EntityFrameworkCore;
using StatisticDbContext.Models;

namespace StatisticDbContext.Seeds
{
    public static class RecipesSeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipeModel>().HasData(
                new RecipeModel() { id = 1, item_id = 1, crafting_station = "forge", craft_time = 30 },
                new RecipeModel() { id = 2, item_id = 2, crafting_station = "forge", craft_time = 30 },
                new RecipeModel() { id = 3, item_id = 3, crafting_station = "forge", craft_time = 30 },
                new RecipeModel() { id = 4, item_id = 4, crafting_station = "forge", craft_time = 30 },
                new RecipeModel() { id = 5, item_id = 5, crafting_station = "forge", craft_time = 30 }
                );
        }
    }
}
