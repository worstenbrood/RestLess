using System.Linq;
using RestLess.OData.Interfaces;

namespace RestLess.OData.Filter
{
    public class ConditionJoiner : ConditionBase<string>
    {
        private const string Separator = " ";

        public ConditionJoiner(params ICondition[] conditions) : base(string.Join(Separator, conditions.Select(c => c.ToString())))
        {
        }
    }
}
