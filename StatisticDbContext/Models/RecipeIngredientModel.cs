using System.Text.Json.Serialization;

namespace StatisticDbContext.Models
{
    public class RecipeIngredientModel
    {
        public int recipe_id { get; set; }
        [JsonIgnore]
        public RecipeModel recipe { get; set; }

        public int ingredient_id { get; set; }
        public ItemModel ingredient { get; set; }

        public int quantity { get; set; }
    }
}
