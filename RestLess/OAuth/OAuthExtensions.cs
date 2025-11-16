using System.Collections.Generic;

namespace RestLess.OAuth
{
    public static class Extensions
    {
        public static Dictionary<string, string> AddIfNotEmpty(this Dictionary<string, string> dictionary, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                dictionary[key] = value;
            }

            return dictionary;
        }
    }
}
