namespace HuntingService.Models
{
    public class GameResult
    {
        public string? GameId { get; set; }
        public Int32 Aims { get; set; }
        public Int32 Shots { get; set; }
        public IEnumerable<AimCountModel>? AimCount { get; set; }

        public class AimCountModel
        {
            public Int32 id { get; set; }
            public Int32 Count { get; set; }
        }
    }
}
