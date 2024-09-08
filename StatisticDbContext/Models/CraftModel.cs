namespace StatisticDbContext.Models
{
    public class CraftModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int recire_id { get; set; }
        public RecipeModel recipe { get; set; } = null!;
        public DateTime dt_end { get; set; }
    }
}
