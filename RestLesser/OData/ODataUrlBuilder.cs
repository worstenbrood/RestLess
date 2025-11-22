using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RestLesser.OData
{
    /// <summary>
    /// UrlBuilder with specific logic for OData
    /// </summary>
    /// <remarks>
    /// Constructor
    /// </remarks>
    /// <param name="path"></param>
    public class ODataUrlBuilder<TClass>(string path) : UrlBuilder<ODataUrlBuilder<TClass>, 
        ODataQueryBuilder<TClass>>(path, new ODataQueryBuilder<TClass>(path))
    {
        /// <summary>
        /// Add $select
        /// </summary>
        public ODataUrlBuilder<TClass> Select(params Expression<Select<TClass>>[] selectors)
        {
            QueryBuilder.Select(selectors);
            return this;
        }

        /// <summary>
        /// Select all properties
        /// </summary>
        public ODataUrlBuilder<TClass> SelectAll()
        {
            QueryBuilder.SelectAll();
            return this;
        }

        /// <summary>
        /// Add $expand
        /// </summary>
        public ODataUrlBuilder<TClass> Expand(params Expression<Expand<TClass>>[] expands)
        {
            QueryBuilder.Expand(expands);
            return this;
        }

        /// <summary>
        /// Add $expand
        /// </summary>
        public ODataUrlBuilder<TClass> Filter<TProperty>(Expression<Func<TClass, TProperty>> field, 
            FunctionF<TClass, TProperty> condition)
        {
            QueryBuilder.Filter(field, condition);
            return this;
        }

        /// <summary>
        /// Add " and " to the filter condition
        /// </summary>
        /// <returns></returns>
        public ODataUrlBuilder<TClass> And()
        {
            QueryBuilder.And();
            return this;
        }

        /// <summary>
        /// Add " or " to the filter condition
        /// </summary>
        /// <returns></returns>
        public ODataUrlBuilder<TClass> Or()
        {
            QueryBuilder.Or();
            return this;
        }

        /// <summary>
        /// Add $top
        /// </summary>
        public ODataUrlBuilder<TClass> Top(int count)
        {
            QueryBuilder.Top(count);
            return this;
        }

        /// <summary>
        /// Add $skip
        /// </summary>
        public ODataUrlBuilder<TClass> Skip(int count)
        {
            QueryBuilder.Skip(count);
            return this;
        }

        /// <summary>
        /// Add $orderby (ascending)
        /// </summary>
        public ODataUrlBuilder<TClass> OrderBy(params Expression<Select<TClass>>[] fields)
        {
            QueryBuilder.OrderBy(fields);
            return this;
        }

        /// <summary>
        /// Add $orderby (descending)
        /// </summary>
        public ODataUrlBuilder<TClass> OrderByDescending(Expression<Select<TClass>> field)
        {
            QueryBuilder.OrderByDescending(field);
            return this;
        }

        /// <summary>
        /// Reset query
        /// </summary>
        /// <returns></returns>
        public ODataUrlBuilder<TClass> ResetQuery()
        {
            Reset();
            return this;
        }
    }
}
