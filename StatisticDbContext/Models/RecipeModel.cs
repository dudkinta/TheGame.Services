namespace StatisticDbContext.Models
{
    public class RecipeModel
    {
        public int id { get; set; }

        public int item_id { get; set; }
        public ItemModel? item { get; set; }

        public int craft_time { get; set; }

        public string crafting_station { get; set; } = string.Empty;

        public ICollection<RecipeIngredientModel> ingredients { get; set; } = new List<RecipeIngredientModel>();
    }
}
