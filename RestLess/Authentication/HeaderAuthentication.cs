using System.Net.Http;

namespace RestLess.Authentication
{
    /// <summary>
    /// Authenticate using custom http header/value.
    /// </summary>
    public class HeaderAuthentication : IAuthentication
    {
        private readonly string _header;
        private readonly string _value;

        public HeaderAuthentication(string header, string value)
        {
            _header = header;
            _value = value;
        }

        public void SetAuthentication(HttpRequestMessage request)
        {
            request.Headers.Add(_header, _value);
        }
    }
}
