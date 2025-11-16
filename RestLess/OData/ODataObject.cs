using Newtonsoft.Json;
using RestLess.OData.Models;

namespace RestLess.OData
{
    /// <summary>
    /// OData base object, contains the metadata object
    /// </summary>
    public class ODataObject
    {
        [JsonProperty("__metadata")]
        public Metadata Metadata { get; set; }
    }
}
