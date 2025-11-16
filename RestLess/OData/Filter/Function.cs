using RestLesser.OData;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace RestLesser.OData.Filter
{
    /// <summary>
    /// Function methods
    /// </summary>
    public enum Method
    {
        /// <summary>
        /// None
        /// </summary>
        None,

        /// <summary>
        /// Contains
        /// </summary>
        Contains,

        /// <summary>
        /// EndsWith
        /// </summary>
        EndsWith,

        /// <summary>
        /// StartsWith
        /// </summary>
        StartsWith,

        /// <summary>
        /// ToLower
        /// </summary>
        ToLower,

        /// <summary>
        /// ToUpper
        /// </summary>
        ToUpper,

        /// <summary>
        /// Substring
        /// </summary>
        Substring
    }

    /// <summary>
    /// Function 
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class Function<TClass, TProperty> : Condition<TClass, TProperty, Method>
    {
        private readonly Method _method;
        
        /// <summary>
        /// Default format string
        /// </summary>
        private string DefaultFormat => $"{Operation}({{0}},{{2}})";

        /// <summary>
        /// Conditional format string (ToLower, ToUpper)
        /// </summary>
        private string ConditionFormat => $"{Operation}({{0}})";

        /// <inheritdoc/>
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        public Function(Expression<Func<TClass, TProperty>> field, Method operation, 
            TProperty value) : base(field, operation, value)
        {
            _method = operation;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        public Function(Expression<Func<TClass, TProperty>> field, Method operation, 
            TProperty[] value = null) : base(field, operation, value)
        {
            _method = operation;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        public Function(Method operation, TProperty[] value = null) : base(null, operation, value)
        {
            _method = operation;
        }

        private string[] _parameters;

        /// <summary>
        /// Set function parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Function<TClass, TProperty> SetParameters(params object[] parameters)
        {
            _parameters = parameters
                .Select(p => p.ToODataValue())
                .ToArray();

            return this;
        }

        /// <summary>
        /// Return string representation of this Function
        /// </summary>
        /// <returns></returns>
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
