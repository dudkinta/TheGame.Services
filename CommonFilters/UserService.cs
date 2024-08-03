using System.Security.Claims;

namespace CommonLibs
{
    public class UserService : IUserService
    {
        public long GetTelegramId(IEnumerable<Claim> claims)
        {
            var tg_id = claims.FirstOrDefault(_ => _.Type == "tg_id")?.Value;
            if (string.IsNullOrEmpty(tg_id))
                throw new Exception("Telegram user is corrupt");

            long parse_id = 0;
            if (!long.TryParse(tg_id, out parse_id))
                throw new Exception("Telegram user is corrupt");

            return parse_id;
        }

        public int GetUserId(IEnumerable<Claim> claims)
        {
            var userIdStr = claims.FirstOrDefault(_ => _.Type == "id")?.Value;
            var userId = 0;
            if (string.IsNullOrEmpty(userIdStr))
                throw new Exception("userId not found");

            if (!Int32.TryParse(userIdStr, out userId))
                throw new Exception("userId is bad");

            return userId;
        }


    }
}
