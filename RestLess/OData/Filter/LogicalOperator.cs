using System;

namespace RestLess.OData.Filter
{
    public class LogicalOperator<TOperator> : ConditionBase<TOperator>
        where TOperator: struct, Enum
    {
        public LogicalOperator(TOperator op) : base(op.Lower())
        {    
        }
    }

    public class LogicalOperator : LogicalOperator<Operator>
    {
        public LogicalOperator(Operator op) : base(op)
        {
        }
    }
}
