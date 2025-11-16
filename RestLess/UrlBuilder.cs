using System;
using System.Collections.Generic;

namespace RestLesser
{
    /// <summary>
    /// Url builder base
    /// </summary>

    public abstract class UrlBuilder<TUrl, TQuery> : UriBuilder
        where TUrl : UrlBuilder<TUrl, TQuery>
        where TQuery : QueryBuilder
    {
        /// <summary>
        /// Query builder
        /// </summary>
        protected readonly TQuery QueryBuilder;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="queryBuilder"></param>
        protected UrlBuilder(string path, TQuery queryBuilder) : base(new RelativeUri(path))
        {
            QueryBuilder = queryBuilder;
        }

        /// <summary>
        /// Set a query parameter
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public TUrl SetQueryParameter(string key, string value)
        {
            QueryBuilder?.SetQueryParameter(key, value);
            return this as TUrl;
        }

        /// <summary>
        /// Set multiple query parameters
        /// </summary>
        public TUrl SetQueryParameters(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            QueryBuilder?.SetQueryParameters(keyValuePairs);
            return this as TUrl;
        }

        /// <summary>
        /// Reset builder
        /// </summary>
        public virtual void Reset()
        {
            QueryBuilder?.Reset();
        }

        /// <summary>
        /// Build path and query
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var query = QueryBuilder?.ToString();
            
            // Add query if any
            if (!string.IsNullOrEmpty(query))
            {
                return $"{Path}?{query}";
            }

            return Path;
        }
    }

    /// <summary>
    /// Url builder
    /// </summary>
    public class UrlBuilder : UrlBuilder<UrlBuilder, QueryBuilder>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path"></param>
        public UrlBuilder(string path) : base(path, new QueryBuilder(path))
        {
        }
    }
}
