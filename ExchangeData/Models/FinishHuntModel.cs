namespace ExchangeData.Models
{
    public class FinishHuntModel
    {
        public int Id { get; set; }
        public int AddShots { get; set; }
        public int AddAims { get; set; }
        public int coins { get; set; }
        public IEnumerable<ItemModel>? Items { get; set; }
        public IEnumerable<HeroModel>? Heroes { get; set; }
    }
}
