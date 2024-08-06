using Consul;
using System.Text;

namespace CommonLibs
{
    public class JwtSecretProvider
    {
        private readonly IConsulClient _consulClient;

        public JwtSecretProvider(IConsulClient consulClient)
        {
            _consulClient = consulClient;
        }

        public async Task<string> GetSecretKeyAsync(string key)
        {
            var getPair = await _consulClient.KV.Get(key);
            if (getPair.Response == null)
            {
                throw new Exception("Key not found in Consul");
            }
            return Encoding.UTF8.GetString(getPair.Response.Value, 0, getPair.Response.Value.Length);
        }

        public string GetSecretKey(string key)
        {
            return GetSecretKeyAsync("thegame/" + key).GetAwaiter().GetResult();
        }
    }
}
