namespace ExchangeData.Models
{
    public class HuntArmyModel
    {
        public HeroModel? Hero { get; set; }
        public ItemModel? Gun { get; set; }

        public IEnumerable<ItemModel>? RewardsItems { get; set; }
        public IEnumerable<HeroModel>? RewardsHeroes { get; set; }
    }
}
