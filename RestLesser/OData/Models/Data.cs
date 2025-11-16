using Newtonsoft.Json;

namespace RestLesser.OData.Models
{
    /// <summary>
    /// OData result data
    /// </summary>
    /// <typeparam name="T">Type of the result</typeparam>
    public class Data<T>
    {
        /// <summary>
        /// Results
        /// </summary>
        [JsonProperty("results")]
        public T Results { get; set; }
    }
}
