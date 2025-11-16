using Newtonsoft.Json;

namespace RestLess.OData.Models
{
    /// <summary>
    /// OData object metadata
    /// </summary>
    public class Metadata
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
