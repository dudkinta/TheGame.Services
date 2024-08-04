using System.Text.Json.Serialization;

namespace StatisticDbContext.Models
{
    public class InventoryModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int item_id { get; set; }
        public ItemModel? item { get; set; }
        public int? army_id { get; set; }
        [JsonIgnore]
        public ArmyModel? army { get; set; }
    }
}
