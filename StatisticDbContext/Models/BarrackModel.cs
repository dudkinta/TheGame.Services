namespace StatisticDbContext.Models
{
    public class BarrackModel
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public int hero_id { get; set; }
        public HeroModel? hero { get; set; }
        public ArmyModel? army { get; set; }
    }
}
