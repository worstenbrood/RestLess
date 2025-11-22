using System.Linq;
using RestLesser.OData.Interfaces;

namespace RestLesser.OData.Filter
{
    /// <summary>
    /// Operation class
    /// </summary>
    public class Operation : Collector<string>
    {   
        /// <summary>
        /// Constructor
        /// </summary>
        public Operation() : base(Constants.Query.ConditionSeparator)
        { 
        }

         /// <summary>
        /// Add a condition
        /// </summary>
        /// <param name="condition"></param>
        public void Add(ICondition condition)
        {
            Add(condition.ToString());
        }

        /// <summary>
        /// Add multiple conditions
        /// </summary>
        /// <param name="conditions"></param>
        public void Add(params ICondition[] conditions)
        {
            AddRange(conditions.Select(c => c.ToString()));
        }

        /// <summary>
        /// Reset conditions
        /// </summary>
        public void Reset() => Clear();
    }
}
