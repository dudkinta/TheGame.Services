using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginDbContext.Models
{
    public class UserModel
    {
        public int id { get; set; }

        public long tg_id { get; set; }

        public bool allows_write_to_pm { get; set; }

        public string? first_name { get; set; }

        public bool is_premium { get; set; }

        public string? language_code { get; set; }

        public string? last_name { get; set; }

        public string? username { get; set; }

        public DateTime last_login { get; set; }
    }
}
