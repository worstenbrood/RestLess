using System;
using System.Linq;
using System.Linq.Expressions;

namespace RestLess.OData.Filter
{
    public enum Method
    {
        None,
        Contains,
        EndsWith,
        StartsWith,
        ToLower,
        ToUpper,
        Substring
    }

    public class Function<TClass, TProperty> : Condition<TClass, TProperty, Method>
    {
        private readonly Method _method;
        
        private string DefaultFormat => $"{Operation}({{0}},{{2}})";
        private string ConditionFormat => $"{Operation}({{0}})";

        protected override string Format => _method switch
        {
            Method.Contains => DefaultFormat,
            Method.EndsWith => DefaultFormat,
            Method.StartsWith => DefaultFormat,
            Method.ToLower => ConditionFormat,
            Method.ToUpper => ConditionFormat,
            Method.None => base.ToString(),
            _ => string.Empty,
        };        

        public Function(Expression<Func<TClass, TProperty>> field, Method operation, 
            TProperty value) : base(field, operation, value)
        {
            _method = operation;
        }

        public Function(Expression<Func<TClass, TProperty>> field, Method operation, 
            TProperty[] value = null) : base(field, operation, value)
        {
            _method = operation;
        }

        public Function(Method operation, TProperty[] value = null) : base(null, operation, value)
        {
            _method = operation;
        }

        private string[] _parameters;

        public Function<TClass, TProperty> SetParameters(params object[] parameters)
        {
            _parameters = parameters
                .Select(p => p.ToODataValue())
                .ToArray();

            return this;
        }

        public override string ToString()
        {
            if (_parameters == null || _parameters.Length == 0)
            {
                return base.ToString();
            }

            var parameters = string.Join(",", _parameters);

            switch(_method)
            {
                case Method.Substring:
                    return $"{Operation}({Field},{parameters})";
                default:
                    return base.ToString();
            }
        }
    }
}
