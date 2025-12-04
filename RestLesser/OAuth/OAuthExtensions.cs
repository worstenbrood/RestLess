using System.Collections.Generic;

namespace RestLesser.OAuth
{
    /// <summary>
    /// OAuth extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Adds a key value pair if the value is not null or empty
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<string, string> TryAddValue(this Dictionary<string, string> dictionary, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                dictionary[key] = value;
            }

            return dictionary;
        }
    }
}
