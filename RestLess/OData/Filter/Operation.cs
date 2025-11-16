using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RestLess.OData.Interfaces;

namespace RestLess.OData.Filter
{
    public class Operation : IEnumerable<string>
    {
        private const string Separator = " ";

        private readonly List<string> _conditions = new List<string>();

        public int Count => _conditions.Count;

        public void Add(ICondition condition)
        {
            _conditions.Add($"{condition}");
        }

        public void AddRange(ICondition[] conditions)
        {
            _conditions.AddRange(conditions.Select(c => $"{c}"));
        }

        public void Reset() => _conditions.Clear();

        public override string ToString()
        {
            // Join conditions
            return string.Join(Separator, _conditions);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _conditions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _conditions.GetEnumerator();
        }
    }
}
