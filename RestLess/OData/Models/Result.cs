using Newtonsoft.Json;

namespace RestLess.OData.Models
{
    /// <summary>
    /// OData result
    /// </summary>
    /// <typeparam name="T">Type of the result</typeparam>
    public class Result<T>
    {
        [JsonProperty("d")]
        public Data<T> Data { get; set; }
    }
}
