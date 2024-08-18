namespace StatisticDbContext.Models
{
    public class RecipeIngredientModel
    {
        public int RecipeID { get; set; }
        public RecipeModel Recipe { get; set; }

        public int IngredientItemID { get; set; }
        public ItemModel IngredientItem { get; set; }

        public int Quantity { get; set; }
    }
}
