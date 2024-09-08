using Microsoft.EntityFrameworkCore;
using StatisticDbContext.Models;

namespace StatisticDbContext.Seeds
{
    public static class RecipeIngredientSeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipeIngredientModel>().HasData(
                new RecipeIngredientModel() { recipe_id = 1, quantity = 1, ingredient_id = 20 },
                new RecipeIngredientModel() { recipe_id = 2, quantity = 1, ingredient_id = 1 },
                new RecipeIngredientModel() { recipe_id = 2, quantity = 1, ingredient_id = 20 },
                new RecipeIngredientModel() { recipe_id = 3, quantity = 1, ingredient_id = 2 },
                new RecipeIngredientModel() { recipe_id = 3, quantity = 1, ingredient_id = 20 },
                new RecipeIngredientModel() { recipe_id = 3, quantity = 1, ingredient_id = 21 },
                new RecipeIngredientModel() { recipe_id = 4, quantity = 1, ingredient_id = 3 },
                new RecipeIngredientModel() { recipe_id = 4, quantity = 1, ingredient_id = 20 },
                new RecipeIngredientModel() { recipe_id = 4, quantity = 1, ingredient_id = 21 },
                new RecipeIngredientModel() { recipe_id = 5, quantity = 1, ingredient_id = 4 },
                new RecipeIngredientModel() { recipe_id = 5, quantity = 1, ingredient_id = 20 },
                new RecipeIngredientModel() { recipe_id = 5, quantity = 1, ingredient_id = 21 }
                );
        }
    }
}
