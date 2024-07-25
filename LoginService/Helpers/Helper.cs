using System.Security.Cryptography;
using System.Text;

namespace LoginService.Helpers
{
    public static class Helper
    {
        public static bool Validate(string hashStr, IEnumerable<string[]> initDataList, string token, string cStr = "WebAppData")
        {
            var initDataString = string.Join("\n", initDataList.Select(rec => $"{rec[0]}={rec[1]}"));

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(cStr));
            var secretKey = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));

            using var dataCheckHmac = new HMACSHA256(secretKey);
            var dataCheck = dataCheckHmac.ComputeHash(Encoding.UTF8.GetBytes(initDataString));
            var dataCheckHex = BitConverter.ToString(dataCheck).Replace("-", "").ToLower();
            return dataCheckHex == hashStr;
        }
    }
}
