namespace LoginService.Models
{
    public class TWAUnsafeInitData
    {
        public int? auth_date { get; set; }

        public long? chat_instance { get; set; }

        public string? chat_type { get; set; }

        public string? hash { get; set; }

        public string? start_param { get; set; }

        public TWAUserModel? user { get; set; }
    }
}
