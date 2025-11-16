using RestLesser;
using System.Net.Http;

namespace RestLesser.Authentication
{
    /// <summary>
    /// Authenticate with a custom url query parameter and value
    /// </summary>
    public class UrlAuthentication : IAuthentication
    {
        private readonly string _key;
        private readonly string _value;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key">Url query parameter</param>
        /// <param name="value">Value</param>
        public UrlAuthentication(string key, string value)
        {
            _key = key;
            _value = value;
        }

        /// <inheritdoc/>
        public void SetAuthentication(HttpRequestMessage request)
        {
            request.SetUrlAuthentication(_key, _value);
        }
    }
}
