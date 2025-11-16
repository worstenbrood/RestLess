using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using RestLesser.OData.Interfaces;

namespace RestLesser.OData.Filter
{
    /// <summary>
    /// Condition base class
    /// </summary>
    /// <typeparam name="TClass">Class type</typeparam>
    /// <typeparam name="TProperty">Property type</typeparam>
    /// <typeparam name="TEnum">Enum type</typeparam>
    public abstract class Condition<TClass, TProperty, TEnum> : ICondition
        where TEnum : struct, Enum
    {
        /// <summary>
        /// Field string
        /// </summary>
        protected readonly string Field;

        /// <summary>
        /// Operation string
        /// </summary>
        protected readonly string Operation;

        /// <summary>
        /// Value string
        /// </summary>
        protected readonly string Value;

        /// <summary>
        /// Generates a format string, using only args that are not null
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected static string GenerateFormat(string separator, params object[] args)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg == null)
                {
                    continue;
                }

                sb.AppendFormat("{{{0}}}", i);
                
                if (i < args.Length - 1)
                {
                    sb.Append(separator);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// {0} = field
        /// {1} = operation/function
        /// {2} = value
        /// </summary>
        protected virtual string Format => GenerateFormat(" ", Field, Operation, Value);

        /// <summary>
        /// Return formatted string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(Format, Field, Operation, Value);
        }

        private Condition(Expression<Func<TClass, TProperty>> field, TEnum operation)
        {
            Field = field?.GetMemberName();
            Operation = operation.Lower();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        public Condition(Expression<Func<TClass, TProperty>> field, TEnum operation,
            TProperty value) : this(field, operation)
        {
            Value = value?.ToODataValue();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        public Condition(Expression<Func<TClass, TProperty>> field, TEnum operation,
            TProperty[] value) : this(field, operation)
        {
            if (value != null)
            {
                Value = $"({string.Join(",", value.Select(v => v.ToODataValue()))})";
            }
        }
    }

    /// <summary>
    /// Operators
    /// </summary>
    public enum Operator
    {
        /// <summary>
        /// No operator
        /// </summary>
        None,

        /// <summary>
        /// Equals
        /// </summary>
        Eq,

        /// <summary>
        /// Greater then
        /// </summary>
        Gt,

        /// <summary>
        /// Less then
        /// </summary>
        Lt,

        /// <summary>
        /// In
        /// </summary>
        In,

        /// <summary>
        /// Greater then or equal to
        /// </summary>
        Ge,

        /// <summary>
        /// Less then or equal to
        /// </summary>
        Le,

        /// <summary>
        /// Different from
        /// </summary>
        Ne,

        /// <summary>
        /// And
        /// </summary>
        And,

        /// <summary>
        /// Or
        /// </summary>
        Or
    }

    /// <summary>
    /// Condition using <see cref="Operator"/>
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    public class Condition<TClass, TProperty> : Condition<TClass, TProperty, Operator>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        public Condition(Expression<Func<TClass, TProperty>> field, Operator operation,
            TProperty value) : base(field, operation, value)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field"></param>
        /// <param name="operation"></param>
        /// <param name="value"></param>
        public Condition(Expression<Func<TClass, TProperty>> field, Operator operation,
            TProperty[] value) : base(field, operation, value)
        {
        }
    }
}