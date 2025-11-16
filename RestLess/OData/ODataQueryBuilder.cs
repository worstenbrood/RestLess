using System;
using System.Collections;
using System.Linq.Expressions;
using RestLesser.OData.Filter;
using RestLesser.OData.Interfaces;

namespace RestLesser.OData
{
    /// <summary>
    /// Select delegate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate object Select<T>(T value);

    /// <summary>
    /// Expand delegate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate IEnumerable Expand<T>(T value);

    /// <summary>
    /// FilterF delegate 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="filterCondition"></param>
    /// <returns></returns>
    public delegate ICondition FilterF<T, U>(ConditionFactory<T, U> filterCondition);

    /// <summary>
    /// Filter delegate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="filterCondition"></param>
    /// <returns></returns>
    public delegate ICondition Filter<T, U>(FunctionFactory<T, U> filterCondition);

    /// <summary>
    /// QueryBuilder with specific logic for odata
    /// </summary>
    public class ODataQueryBuilder<TClass> : QueryBuilder
    {
        private readonly Operation _operation = new Operation();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="query"></param>
        public ODataQueryBuilder(string query) : base(query)
        { 
        }

        /// <summary>
        /// Set multiple expressions
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="key"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        protected ODataQueryBuilder<TClass> SetExpressions<TDelegate>(string key, Expression<TDelegate>[] expressions)
            where TDelegate : class, Delegate
        {
            if (expressions.Length > 0)
            {
                SetQueryParameter(key, expressions.JoinMembers());
            }
            return this;
        }

        /// <summary>
        /// Add $select
        /// </summary>
        /// <param name="selectors"></param>
        public ODataQueryBuilder<TClass> Select(params Expression<Select<TClass>>[] selectors)
        {
            return SetExpressions(Constants.Query.Select, selectors);
        }

        /// <summary>
        /// Add $select
        /// </summary>
        /// <param name="expands"></param>
        public ODataQueryBuilder<TClass> Expand(params Expression<Expand<TClass>>[] expands)
        {
            return SetExpressions(Constants.Query.Expand, expands);
        }

        /// <summary>
        /// Filter on property of TProperty. Strong typed.
        /// </summary>
        /// <typeparam name="TProperty">Main object type</typeparam>
        /// <typeparam name="TFilter">Filter type</typeparam>
        /// <param name="field"></param>
        /// <param name="func"></param>
        /// <param name="filter"></param>
        public ODataQueryBuilder<TClass> Filter<TProperty, TFilter>(
            Expression<Func<TClass, TProperty>> field,
                Filter<TClass, TProperty> func,
                TFilter filter) 
            where TFilter : FunctionFactory<TClass, TProperty>

        {
            var condition = func(filter);

            // Save condition
            _operation.Add(condition);

            // Update the filter
            SetQueryParameter(Constants.Query.Filter, _operation.ToString());
            return this;
        }

        /// <summary>
        /// Filter on property of TProperty using all functions
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="field"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public ODataQueryBuilder<TClass> Filter<TProperty>(
            Expression<Func<TClass, TProperty>> field,
                Filter<TClass, TProperty> func)
        {
            var filter = new FunctionFactory<TClass, TProperty>(field);
            return Filter(field, func, filter);
        }

        /// <summary>
        /// Add " and " to the filter condition
        /// </summary>
        /// <returns></returns>
        public ODataQueryBuilder<TClass> And()
        {
            _operation.Add(new LogicalOperator(Operator.And));
            return this;
        }

        /// <summary>
        /// Add " or " to the filter condition
        /// </summary>
        /// <returns></returns>
        public ODataQueryBuilder<TClass> Or()
        {
            _operation.Add(new LogicalOperator(Operator.Or));
            return this;
        }

        /// <summary>
        /// Set the amount of records you want to return
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public ODataQueryBuilder<TClass> Top(int count)
        {
            if (count > 0)
            {
                SetQueryParameter(Constants.Query.Top, count.ToString());
            }
            return this;
        }

        /// <summary>
        /// Set field to order on (ascending)
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public ODataQueryBuilder<TClass> OrderBy(Expression<Select<TClass>> field)
        {
            SetQueryParameter(Constants.Query.OrderBy, field.GetMemberName());
            return this;
        }

        /// <summary>
        /// Set field to order on (descending)
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public ODataQueryBuilder<TClass> OrderByDescending(Expression<Select<TClass>> field)
        {
            SetQueryParameter(Constants.Query.OrderBy, $"{field.GetMemberName()} {Constants.Query.Desc}");
            return this;
        }

        /// <summary>
        /// Reset query
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            // Also clear conditions
            _operation.Reset();
        }
    }
}
