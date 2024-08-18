namespace StatisticDbContext.Models
{
    public class RecipeModel
    {
        public int RecipeID { get; set; }

        public int ItemID { get; set; }
        public ItemModel? Item { get; set; }

        public int CraftTime { get; set; }

        public string CraftingStation { get; set; } = string.Empty;

        public ICollection<RecipeIngredientModel>? Ingredients { get; set; }
    }
}
