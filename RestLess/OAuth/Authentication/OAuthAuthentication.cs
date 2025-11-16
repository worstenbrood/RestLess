using System.Net.Http;
using System.Net.Http.Headers;
using RestLess.Authentication;
using RestLess.OAuth.Models;
using RestLess.OAuth.Provider;

namespace RestLess.OAuth.Authentication
{
    /// <summary>
    /// IAuthentication implementation of an ITokenProvider for use with RestClient
    /// </summary>
    public class OAuthAuthentication : IAuthentication
    {
        private readonly ITokenProvider _client;

        public OAuthAuthentication(ITokenProvider client)
        {
            _client = client;
        }

        public void SetAuthentication(HttpRequestMessage request)
        {
            TokenResponse response = _client.GetToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);
        }
    }
}
