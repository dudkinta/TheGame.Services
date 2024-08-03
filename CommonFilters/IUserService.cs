using System.Security.Claims;

namespace CommonLibs
{
    public interface IUserService
    {
        int GetUserId(IEnumerable<Claim> claims);
        long GetTelegramId(IEnumerable<Claim> claims);
    }
}
