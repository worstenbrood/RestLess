
using System;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using RestLess.OData.Interfaces;

namespace RestLess.OData.Filter
{
    public abstract class Condition<TClass, TProperty, TEnum> : ICondition
        where TEnum : struct, Enum
    {
        protected readonly string Field;
        protected readonly string Operation;
        protected readonly string Value;

        private static string GenerateFormat(string separator, params object[] args)
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

        public override string ToString()
        {
            return string.Format(Format, Field, Operation, Value);
        }

        private Condition(Expression<Func<TClass, TProperty>> field, TEnum operation)
        {
            Field = field?.GetMemberName();
            Operation = operation.Lower();
        }

        public Condition(Expression<Func<TClass, TProperty>> field, TEnum operation,
            TProperty value) : this(field, operation)
        {
            Value = value?.ToODataValue();
        }

        public Condition(Expression<Func<TClass, TProperty>> field, TEnum operation,
            TProperty[] value) : this(field, operation)
        {
            if (value != null)
            {
                Value = $"({string.Join(",", value.Select(v => v.ToODataValue()))})";
            }
        }
    }

    public enum Operator
    {
        None,
        Eq,
        Gt,
        Lt,
        In,
        Ge,
        Le,
        Ne,
        And,
        Or
    }

    public class Condition<TClass, TProperty> : Condition<TClass, TProperty, Operator>
    {
        public Condition(Expression<Func<TClass, TProperty>> field, Operator operation,
            TProperty value) : base(field, operation, value)
        {
        }

        public Condition(Expression<Func<TClass, TProperty>> field, Operator operation,
            TProperty[] value) : base(field, operation, value)
        {
        }
    }
}