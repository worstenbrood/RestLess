using RestLess.OData.Interfaces;
using System;
using System.Linq.Expressions;

namespace RestLess.OData.Filter
{
    public class ConditionFactory<TClass, TProperty>
    {
        protected readonly Expression<Func<TClass, TProperty>> Property;
        protected readonly ConditionFactory<TClass, TProperty> Filter;

        public ConditionFactory(Expression<Func<TClass, TProperty>> property = null)
        {
            Property = property;

            // Limited filter functions
            Filter = new ConditionFactory<TClass, TProperty>(property);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public ICondition Eq(TProperty value) => 
            new Condition<TClass, TProperty>(Property, Operator.Eq, value);

        public ICondition Gt(TProperty value) => 
            new Condition<TClass, TProperty>(Property, Operator.Gt, value);

        public ICondition Lt(TProperty value) => 
            new Condition<TClass, TProperty>(Property, Operator.Lt, value);

        public ICondition In(params TProperty[] value) => 
            new Condition<TClass, TProperty>(Property, Operator.In, value);

        public ICondition Ge(TProperty value) => 
            new Condition<TClass, TProperty>(Property, Operator.Ge, value);

        public ICondition Ne(TProperty value) => 
            new Condition<TClass, TProperty>(Property, Operator.Ne, value);

        public ICondition Le(TProperty value) => 
            new Condition<TClass, TProperty>(Property, Operator.Le, value);
    }

    public class FunctionFactory<TClass, TProperty> : ConditionFactory<TClass, TProperty>
    {
        public FunctionFactory(Expression<Func<TClass, TProperty>> property) : base(property)
        {
        }

        public ICondition Contains(TProperty value) =>
            new Function<TClass, TProperty>(Property, Method.Contains, value);

        public ICondition StartsWith(TProperty value) =>
            new Function<TClass, TProperty>(Property, Method.StartsWith, value);

        public ICondition EndsWith(TProperty value) =>
           new Function<TClass, TProperty>(Property, Method.EndsWith, value);

        public ICondition ToLower(FilterF<TClass, TProperty> condition)
        {
            // Prepare function
            var function = new Function<TClass, TProperty>(Property, Method.ToLower);

            // Limited filter functions
            return new ConditionJoiner(function, condition(Filter));
        }

        public ICondition ToUpper(FilterF<TClass, TProperty> condition)
        {
            // Prepare function
            var function = new Function<TClass, TProperty>(Property, Method.ToUpper);

            // Join both conditions
            return new ConditionJoiner(function, condition(Filter));
        }

        public ICondition Substring(int len, FilterF<TClass, TProperty> condition)
        {
            // Prepare function and set parameter 
            var function = new Function<TClass, TProperty>(Property, Method.Substring)
                .SetParameters(len);

            // Join both conditions
            return new ConditionJoiner(function, condition(Filter));
        }
    }
}
