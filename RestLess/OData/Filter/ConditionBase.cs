using RestLesser.OData.Interfaces;

namespace RestLesser.OData.Filter
{
    /// <summary>
    /// Condition base class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConditionBase<T> : ICondition
    {
        /// <summary>
        /// Value
        /// </summary>
        protected readonly string Value;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        public ConditionBase(string value) { Value = value; }

        /// <summary>
        /// Returns <see cref="Value"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value; 
    }
}
