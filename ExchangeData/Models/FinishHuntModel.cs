namespace ExchangeData.Models
{
    public class FinishHuntModel
    {
        public int Id { get; set; }
        public string? Guid { get; set; }
        public int AddShots { get; set; }
        public int AddAims { get; set; }
        public int coins { get; set; }
        public List<ItemModel>? Items { get; set; }
        public List<HeroModel>? Heroes { get; set; }
    }
}
