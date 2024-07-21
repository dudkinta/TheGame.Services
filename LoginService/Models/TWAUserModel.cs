namespace LoginService.Models
{
    public class TWAUserModel
    {
        public long id { get; set; }

        public bool allows_write_to_pm {  get; set; }

        public string? first_name { get; set; }

        public bool is_premium { get; set; }

        public string? language_code { get; set; }

        public string? last_name { get; set; }
        
        public string? username { get; set; }

    }
}
