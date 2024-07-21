using CommonLibs;
using Newtonsoft.Json;

namespace InnerApiLib
{
    public class InnerApiClient : IInnerApiClient
    {
        private readonly ConsulServiceDiscovery _consulServiceDiscovery;
        public InnerApiClient(ConsulServiceDiscovery consulServiceDiscovery)
        {
            _consulServiceDiscovery = consulServiceDiscovery;
        }

        private async Task<HttpClient> GetHttpClient()
        {
            var httpClient = new HttpClient();
            var tokenEndpoint = await _consulServiceDiscovery.GetServiceAddress("InnerTokenService");
            var tokenProvider = new TokenProvider(httpClient, $"http://{tokenEndpoint}/api/inner/token/login");
            var authenticatedHandler = new AuthenticatedHttpClientHandler(tokenProvider)
            {
                InnerHandler = new HttpClientHandler()
            };

            return new HttpClient(authenticatedHandler);
        }

        public async Task<BaseResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
            var client = await GetHttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await client.SendAsync(request, cancellationToken);
            var res = await GetResult<T>(response, cancellationToken);
            return res;
        }

        public async Task<BaseResponse<T>> PostAsync<T>(string url, CancellationToken cancellationToken)
        {
            var client = await GetHttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await client.SendAsync(request, cancellationToken);
            var res = await GetResult<T>(response, cancellationToken);
            return res;
        }

        public async Task<BaseResponse<T>> PostAsync<T>(string url, HttpContent? content, CancellationToken cancellationToken)
        {
            var client = await GetHttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = content;
            var response = await client.SendAsync(request, cancellationToken);
            var res = await GetResult<T>(response, cancellationToken);
            return res;
        }

        public async Task<BaseResponse<T>> PutAsync<T>(string url, CancellationToken cancellationToken)
        {
            var client = await GetHttpClient();
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            var response = await client.SendAsync(request, cancellationToken);
            var res = await GetResult<T>(response, cancellationToken);
            return res;
        }

        public async Task<BaseResponse<T>> PutAsync<T>(string url, HttpContent? content, CancellationToken cancellationToken)
        {
            var client = await GetHttpClient();
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Content = content;
            var response = await client.SendAsync(request, cancellationToken);
            var res = await GetResult<T>(response, cancellationToken);
            return res;
        }

        public async Task<BaseResponse<T>> DeleteAsync<T>(string url, CancellationToken cancellationToken)
        {
            var client = await GetHttpClient();
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            var response = await client.SendAsync(request, cancellationToken);
            var res = await GetResult<T>(response, cancellationToken);
            return res;
        }

        private static async Task<BaseResponse<T>> GetResult<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            string responseStr = await response.Content.ReadAsStringAsync(cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                if (!string.IsNullOrEmpty(responseStr))
                {
                    return new BaseResponse<T>()
                    {
                        IsSuccessStatusCode = response.IsSuccessStatusCode,
                        StatusCode = response.StatusCode,
                        Error = null,
                        Message = JsonConvert.DeserializeObject<T>(responseStr)
                    };
                }
            }
            return new BaseResponse<T>()
            {
                IsSuccessStatusCode = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Error = JsonConvert.DeserializeObject<ErrorResponse>(responseStr),
            };
        }
    }
}
