using System;
using System.Web;
using System.Net.Http;
using System.Collections.Specialized;

namespace RestLess.Authentication
{
    /// <summary>
    /// Authenticate with a custom url query parameter and value
    /// </summary>
    public class UrlAuthentication : IAuthentication
    {
        private readonly string _key;
        private readonly string _value;

        public UrlAuthentication(string key, string value)
        {
            _key = key;
            _value = value;
        }

        public void SetAuthentication(HttpRequestMessage request)
        {
            NameValueCollection httpValueCollection = HttpUtility.ParseQueryString(request.RequestUri.Query);
            httpValueCollection.Add(_key, _value);

            UriBuilder ub = new UriBuilder(request.RequestUri)
            {
                Query = httpValueCollection.ToString()
            };

            request.RequestUri = ub.Uri;
        }
    }
}
