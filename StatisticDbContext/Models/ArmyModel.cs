namespace StatisticDbContext.Models
{
    public class ArmyModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int barrack_id { get; set; }
        public BarrackModel? hero { get; set; }
        public IEnumerable<InventoryModel>? equip { get; set; }
    }
}
