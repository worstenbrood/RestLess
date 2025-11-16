using RestLesser;
using System.Net.Http;

namespace RestLesser.Authentication
{
    /// <summary>
    /// Authentication using a Bearer token
    /// </summary>
    public class TokenAuthentication : IAuthentication
    {
        private readonly string _token;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token">Bearer token</param>
        public TokenAuthentication(string token)
        {
            _token = token;
        }

        /// <inheritdoc/>
        public void SetAuthentication(HttpRequestMessage request)
        {
            request.SetToken(_token);
        }
    }
}
