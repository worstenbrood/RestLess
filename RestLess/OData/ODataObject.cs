using Newtonsoft.Json;
using RestLesser.OData.Models;

namespace RestLesser.OData
{
    /// <summary>
    /// OData base object, contains the metadata object
    /// </summary>
    public class ODataObject
    {
        /// <summary>
        /// Meta data
        /// </summary>
        [JsonProperty("__metadata")]
        public Metadata Metadata { get; set; }
    }
}
