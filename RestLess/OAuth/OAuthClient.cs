using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using RestLesser;
using RestLesser.Authentication;
using RestLesser.OAuth.Models;

namespace RestLesser.OAuth
{
    /// <summary>
    /// OAuth rest client
    /// </summary>
    public class OAuthClient : RestClient
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Endpoint Url</param>
        public OAuthClient(string endPoint) : base(endPoint)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Endpoint Url</param>
        /// <param name="handler"></param>
        public OAuthClient(string endPoint, HttpClientHandler handler) : base(endPoint, handler)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Endpoint Url</param>
        /// <param name="authentication"></param>
        /// <param name="handler"></param>
        public OAuthClient(string endPoint, IAuthentication authentication, HttpClientHandler handler) : base(endPoint, authentication, handler)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Endpoint Url</param>
        /// <param name="certificate"></param>
        /// <param name="authentication"></param>
        public OAuthClient(string endPoint, X509Certificate certificate, IAuthentication authentication) : base(endPoint, certificate, authentication)
        {
        }

        /// <inheritdoc/>
        protected override async Task<string> HandleResponse(HttpResponseMessage response)
        {
            string body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return body;
            }

            var error = DataAdapter.Deserialize<ErrorResponse>(body);
            throw new HttpRequestException($"HTTP Status {response.StatusCode}: {error.Error}");
        }
    }
}
