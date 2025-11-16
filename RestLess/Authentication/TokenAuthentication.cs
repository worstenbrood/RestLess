using System.Net.Http;
using System.Net.Http.Headers;

namespace RestLess.Authentication
{
    public class TokenAuthentication : IAuthentication
    {
        private readonly string _token;

        public TokenAuthentication(string token)
        {
            _token = token;
        }

        public void SetAuthentication(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }
    }
}
