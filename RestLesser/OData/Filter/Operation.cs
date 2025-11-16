using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RestLesser.OData.Interfaces;

namespace RestLesser.OData.Filter
{
    /// <summary>
    /// Operation class
    /// </summary>
    public class Operation : IEnumerable<string>
    {
        private const string Separator = " ";
                
        private readonly List<string> _conditions = new List<string>();


        /// <summary>
        /// Return condition count
        /// </summary>
        public int Count => _conditions.Count;

        /// <summary>
        /// Add a condition
        /// </summary>
        /// <param name="condition"></param>
        public void Add(ICondition condition)
        {
            _conditions.Add($"{condition}");
        }

        /// <summary>
        /// Add multiple conditions
        /// </summary>
        /// <param name="conditions"></param>
        public void Add(params ICondition[] conditions)
        {
            _conditions.AddRange(conditions.Select(c => $"{c}"));
        }

        /// <summary>
        /// Reset conditions
        /// </summary>
        public void Reset() => _conditions.Clear();

        /// <summary>
        /// Return condition string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Join conditions
            return string.Join(Separator, _conditions);
        }

        /// <inheritdoc/>
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
