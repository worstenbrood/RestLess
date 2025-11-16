using System.Net.Http;

namespace RestLesser.Authentication
{
    /// <summary>
    /// Authenticate using custom http header/value.
    /// </summary>
    public class HeaderAuthentication : IAuthentication
    {
        private readonly string _header;
        private readonly string _value;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Header</param>
        /// <param name="value">Value (token)</param>
        public HeaderAuthentication(string header, string value)
        {
            _header = header;
            _value = value;
        }

        /// <inheritdoc/>
        public void SetAuthentication(HttpRequestMessage request)
        {
            request.SetHeader(_header, _value);
        }
    }
}
