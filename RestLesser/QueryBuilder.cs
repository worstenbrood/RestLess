using System.Web;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace RestLesser
{
    /// <summary>
    /// Url query builder
    /// </summary>
    public class QueryBuilder
    {
        /// <summary>
        /// Query parameters
        /// </summary>
        protected readonly NameValueCollection QueryParameters;

        /// <summary>
        /// Relative url
        /// </summary>
        protected readonly RelativeUri RelativeUrl;

        /// <summary>
        /// PathAndQuery
        /// </summary>
        /// <param name="path"></param>
        public QueryBuilder(string path)
        {
            RelativeUrl = new RelativeUri(path);
            QueryParameters = HttpUtility.ParseQueryString(RelativeUrl.Query);
        }

        /// <summary>
        /// No existing query
        /// </summary>
        public QueryBuilder() : this(string.Empty)
        {
        }

        /// <summary>
        /// Set query parameter and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public QueryBuilder SetQueryParameter(string key, string value)
        {
            QueryParameters[key] = value;
            return this;
        }

        /// <summary>
        /// Set query parameters and values
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <returns></returns>
        public QueryBuilder SetQueryParameters(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            foreach (var kv in keyValuePairs)
            {
                QueryParameters[kv.Key] = kv.Value;
            }
            return this;
        }

        /// <summary>
        /// Clear query parameters
        /// </summary>
        public virtual void Reset() => QueryParameters.Clear();


        /// <summary>
        /// Get encoded query string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => QueryParameters?.ToString() ?? base.ToString();
    }
}
