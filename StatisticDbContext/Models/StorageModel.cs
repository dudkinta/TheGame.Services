namespace StatisticDbContext.Models
{
    public class StorageModel
    {
        public int id { get; set; }
        public int energy { get; set; }
        public DateTime last_check_energy { get; set; }
        public long main_coin { get; set; }
        public long refer_coin { get; set; }
        public long task_coin { get; set; }
        public long bonus_coin { get; set; }
        public long aim { get; set; }
        public long hunts { get; set; }
        public long shots { get; set; }
    }
}
