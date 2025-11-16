using System;

namespace RestLesser.OData.Filter
{
    /// <summary>
    /// Logical operator base class
    /// </summary>
    /// <typeparam name="TOperator"></typeparam>
    public class LogicalOperator<TOperator> : ConditionBase<TOperator>
        where TOperator: struct, Enum
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="op"></param>
        public LogicalOperator(TOperator op) : base(op.Lower())
        {    
        }
    }

    /// <summary>
    /// Logical operator
    /// </summary>
    public class LogicalOperator : LogicalOperator<Operator>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="op"></param>
        public LogicalOperator(Operator op) : base(op)
        {
        }
    }
}
