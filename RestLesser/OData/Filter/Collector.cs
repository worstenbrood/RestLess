using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RestLesser.OData.Interfaces;

namespace RestLesser.OData.Filter
{
    /// <summary>
    /// Collector
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="separator"></param>
    public class Collector<T>(string separator) : List<T>, ICondition
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Collector() : this(Constants.Query.ParameterSeparator)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args"></param>
        public Collector(params T[] args) : this()
        {
            AddRange(args);
        }

        /// <summary>
        /// Return joined string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join(separator, this);
        }
    }
}
