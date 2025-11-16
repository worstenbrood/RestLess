using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using RestLess.Authentication;
using RestLess.OAuth.Models;

namespace RestLess.OAuth
{
    public class OAuthClient : RestClient
    {
        public OAuthClient(string endPoint) : base(endPoint)
        {
        }

        public OAuthClient(string endPoint, HttpClientHandler handler) : base(endPoint, handler)
        {
        }

        public OAuthClient(string endPoint, IAuthentication authentication, HttpClientHandler handler) : base(endPoint, authentication, handler)
        {
        }

        public OAuthClient(string endPoint, X509Certificate certificate, IAuthentication authentication) : base(endPoint, certificate, authentication)
        {
        }

        protected override string HandleResponse(HttpResponseMessage response)
        {
            string body = response.Content.ReadAsStringAsync().SyncResult();
            if (response.IsSuccessStatusCode)
            {
                return body;
            }

            var error = DataAdapter.Deserialize<ErrorResponse>(body);
            throw new HttpRequestException($"HTTP Status {response.StatusCode}: {error.Error}");
        }
    }
}
