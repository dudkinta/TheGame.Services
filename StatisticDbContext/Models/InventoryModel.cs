namespace StatisticDbContext.Models
{
    public class InventoryModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int item_id { get; set; }
        public ItemModel? item { get; set; }
        public int count { get; set; }
    }
}
