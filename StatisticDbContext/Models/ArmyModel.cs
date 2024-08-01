namespace StatisticDbContext.Models
{
    public class ArmyModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int barrack_id { get; set; }
        public BarrackModel? barrack { get; set; }
        public ICollection<InventoryModel>? equip { get; set; }
        public int useType { get; set; }
    }
}
