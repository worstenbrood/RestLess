using System;
using System.Net.Http.Headers;

namespace RestLesser.DataAdapters
{
    /// <summary>
    /// DataAdapter for primitive types
    /// </summary>
    public class PrimitiveAdapter : IDataAdapter
    {
        /// <inheritdoc/>
        public MediaTypeWithQualityHeaderValue MediaTypeHeader { get; } =
            new MediaTypeWithQualityHeaderValue("*/*");

        /// <inheritdoc/>
        public T Deserialize<T>(string data)
        {
            return (T)Convert.ChangeType(data, typeof(T));
        }

        /// <inheritdoc/>
        public string Serialize<T>(T data)
        {
            return Convert.ToString(data);
        }
    }
}
