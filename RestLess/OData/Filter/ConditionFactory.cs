using RestLess.OData.Interfaces;
using System;
using System.Linq.Expressions;

namespace RestLess.OData.Filter
{
    /// <summary>
    /// Condition factory
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class ConditionFactory<TClass, TProperty>
    {
        /// <summary>
        /// Property
        /// </summary>
        protected readonly Expression<Func<TClass, TProperty>> Property;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="property"></param>
        public ConditionFactory(Expression<Func<TClass, TProperty>> property = null)
        {
            Property = property;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICondition Eq(TProperty value) => 
            new Condition<TClass, TProperty>(Property, Operator.Eq, value);

        /// <summary>
        /// Greater then
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICondition Gt(TProperty value) => 
            new Condition<TClass, TProperty>(Property, Operator.Gt, value);

        /// <summary>
        /// Less then
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICondition Lt(TProperty value) => 
            new Condition<TClass, TProperty>(Property, Operator.Lt, value);

        /// <summary>
        /// In
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICondition In(params TProperty[] value) => 
            new Condition<TClass, TProperty>(Property, Operator.In, value);

        /// <summary>
        /// Greater then or equals to
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICondition Ge(TProperty value) => 
            new Condition<TClass, TProperty>(Property, Operator.Ge, value);

        /// <summary>
        /// Different from
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICondition Ne(TProperty value) => 
            new Condition<TClass, TProperty>(Property, Operator.Ne, value);

        /// <summary>
        /// Less then or equals to
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICondition Le(TProperty value) => 
            new Condition<TClass, TProperty>(Property, Operator.Le, value);
    }

    /// <summary>
    /// Function factory
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class FunctionFactory<TClass, TProperty> : ConditionFactory<TClass, TProperty>
    {
        private readonly ConditionFactory<TClass, TProperty> _after;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="property"></param>
        public FunctionFactory(Expression<Func<TClass, TProperty>> property) : base(property)
        {
            _after = new ConditionFactory<TClass, TProperty>();
        }

        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICondition Contains(TProperty value) =>
            new Function<TClass, TProperty>(Property, Method.Contains, value);

        /// <summary>
        /// StartsWith
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICondition StartsWith(TProperty value) =>
            new Function<TClass, TProperty>(Property, Method.StartsWith, value);

        /// <summary>
        /// EndsWith
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICondition EndsWith(TProperty value) =>
           new Function<TClass, TProperty>(Property, Method.EndsWith, value);

        /// <summary>
        /// ToLower
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ICondition ToLower(FilterF<TClass, TProperty> condition)
        {
            // Prepare function
            var function = new Function<TClass, TProperty>(Property, Method.ToLower);

            // Limited filter functions
            return new ConditionJoiner(function, condition(_after));
        }

        /// <summary>
        /// ToUpper
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ICondition ToUpper(FilterF<TClass, TProperty> condition)
        {
            // Prepare function
            var function = new Function<TClass, TProperty>(Property, Method.ToUpper);

            // Join both conditions
            return new ConditionJoiner(function, condition(_after));
        }

        /// <summary>
        /// Substring
        /// </summary>
        /// <param name="len"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public ICondition Substring(int len, FilterF<TClass, TProperty> condition)
        {
            // Prepare function and set parameter 
            var function = new Function<TClass, TProperty>(Property, Method.Substring)
                .SetParameters(len);

            // Join both conditions
            return new ConditionJoiner(function, condition(_after));
        }
    }
}
