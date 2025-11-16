using System;
using System.Collections;
using System.Linq.Expressions;
using RestLess.OData.Filter;
using RestLess.OData.Interfaces;

namespace RestLess.OData
{
    public delegate object Select<T>(T value);
    public delegate IEnumerable Expand<T>(T value);
    public delegate ICondition FilterF<T, U>(ConditionFactory<T, U> filterCondition);
    public delegate ICondition Filter<T, U>(FunctionFactory<T, U> filterCondition);

    /// <summary>
    /// QueryBuilder with specific logic for odata
    /// </summary>
    public class ODataQueryBuilder<TClass> : QueryBuilder
    {
        private readonly Operation _operation = new Operation();

        public ODataQueryBuilder(string query) : base(query)
        { 
        }

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
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="selectors"></param>
        public ODataQueryBuilder<TClass> Select(params Expression<Select<TClass>>[] selectors)
        {
            return SetExpressions(Constants.Query.Select, selectors);
        }

        /// <summary>
        /// Add $select
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <param name="selectors"></param>
        public ODataQueryBuilder<TClass> Expand(params Expression<Expand<TClass>>[] expands)
        {
            return SetExpressions(Constants.Query.Expand, expands);
        }
       
        /// <summary>
        /// Filter on property of T. Strong typed.
        /// </summary>
        /// <typeparam name="T">Main object type</typeparam>
        /// <typeparam name="U">Parameter type</typeparam>
        /// <param name="selectors"></param>
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

        public override void Reset()
        {
            base.Reset();

            // Also clear conditions
            _operation.Reset();
        }
    }
}
