using System.Net;
using System.Net.Http.Headers;

namespace InnerApiLib
{
    internal class AuthenticatedHttpClientHandler : DelegatingHandler
    {
        private readonly TokenProvider _tokenProvider;

        public AuthenticatedHttpClientHandler(TokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenProvider.GetTokenAsync(cancellationToken));

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenProvider.GetTokenAsync(cancellationToken));
                response = await base.SendAsync(request, cancellationToken);
            }

            return response;
        }
    }
}
