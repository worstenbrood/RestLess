using Newtonsoft.Json;

namespace RestLess.OData.Models
{
    /// <summary>
    /// OData result data
    /// </summary>
    /// <typeparam name="T">Type of the result</typeparam>
    public class Data<T>
    {
        [JsonProperty("results")]
        public T Results { get; set; }
    }
}
