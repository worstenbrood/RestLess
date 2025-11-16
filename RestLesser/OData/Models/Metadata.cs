using Newtonsoft.Json;

namespace RestLesser.OData.Models
{
    /// <summary>
    /// OData object metadata
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Url
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Object type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
