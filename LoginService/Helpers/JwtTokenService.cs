using LoginDbContext.Models;
using LoginService.Interfaces;
using LoginService.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginService.Helpers
{
    public class JwtTokenService : ITokenService
    {
        private readonly string _secretUserKey;

        public JwtTokenService(string secretUserKey)
        {
            _secretUserKey = secretUserKey;
        }

        public string GenerateUserToken(UserModel user, TimeSpan expiration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretUserKey);

            var claims = new List<Claim>
            {
                new Claim("tg_id", user.tg_id.ToString()),
                new Claim("allows_write_to_pm", user.allows_write_to_pm.ToString()),
                new Claim("is_premium", user.is_premium.ToString()),
                new Claim("id", user.id.ToString()),
                new Claim("role", "user")
            };

            if (user.first_name != null)
                claims.Add(new Claim("first_name", user.first_name));

            if (user.username != null)
                claims.Add(new Claim("username", user.username));

            if (user.last_name != null)
                claims.Add(new Claim("last_name", user.last_name));

            if (user.language_code != null)
                claims.Add(new Claim("language_code", user.language_code));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(expiration),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
