using RestLess.OData.Interfaces;

namespace RestLess.OData.Filter
{
    public class ConditionBase<T> : ICondition
    {
        protected readonly string Value;

        public ConditionBase(string value) { Value = value; }

        public override string ToString() => Value; 
    }
}
