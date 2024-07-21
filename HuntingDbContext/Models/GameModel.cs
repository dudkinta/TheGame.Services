using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntingDbContext.Models
{
    public class GameModel
    {
        public Guid game_guid { get; set; }
        public int user_id { get; set; }
        public byte status { get; set; }
        public string? targets { get; set; }
    }
}
