using System.Text.Json.Serialization;

namespace StatisticDbContext.Models
{
    public class ArmyModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int barrack_id { get; set; }
        [JsonIgnore]
        public BarrackModel? barrack { get; set; }
        public ICollection<InventoryModel> equip { get; set; } = new List<InventoryModel>();
        public int useType { get; set; }
    }
}
