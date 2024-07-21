using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CommonLibs
{
    public class JwtBearerOptionsFactory : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly JwtSecretProvider _jwtSecretProvider;

        public JwtBearerOptionsFactory(JwtSecretProvider jwtSecretProvider, IConfiguration configuration)
        {
            _jwtSecretProvider = jwtSecretProvider;
        }

        public void Configure(string? name, JwtBearerOptions options)
        {
            if (name == "User")
            {
                var secretKey = _jwtSecretProvider.GetSecretKey("jwt/user");
                var key = Encoding.ASCII.GetBytes(secretKey);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            }
            else if (name == "Service")
            {
                var secretKey = _jwtSecretProvider.GetSecretKey("jwt/service");
                var key = Encoding.ASCII.GetBytes(secretKey);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            }
        }

        public void Configure(JwtBearerOptions options) { }
    }
}
