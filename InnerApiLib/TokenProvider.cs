using System.Text.Json;
using System.Text.Json.Serialization;

namespace InnerApiLib
{
    internal class TokenProvider
    {
        private string? _token;
        private readonly HttpClient _httpClient;
        private readonly string _tokenEndpoint;

        public TokenProvider(HttpClient httpClient, string tokenEndpoint)
        {
            _httpClient = httpClient;
            _tokenEndpoint = tokenEndpoint;
            _token = string.Empty;
        }

        public async Task<string?> GetTokenAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_token))
            {
                await RefreshTokenAsync(cancellationToken);
            }
            return _token;
        }

        private async Task RefreshTokenAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(_tokenEndpoint, cancellationToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);
            if (tokenResponse != null)
                _token = tokenResponse.Token;
        }

        private class TokenResponse
        {
            [JsonPropertyName("token")]
            public string? Token { get; set; }
        }
    }
}
