using LoginDbContext.Models;
using LoginService.Models;

namespace LoginService.Interfaces
{
    public interface ITokenService
    {
        string GenerateUserToken(UserModel user, TimeSpan expiration);
    }
}
