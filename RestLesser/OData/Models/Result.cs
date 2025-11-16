using Newtonsoft.Json;

namespace RestLesser.OData.Models
{
    /// <summary>
    /// OData result
    /// </summary>
    /// <typeparam name="T">Type of the result</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Data
        /// </summary>
        [JsonProperty("d")]
        public Data<T> Data { get; set; }
    }
}
