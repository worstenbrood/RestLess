using Newtonsoft.Json;

namespace RestLesser.OAuth.Models
{
    /// <summary>
    /// Error response for OAuth
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Error text
        /// </summary>
        [JsonProperty("error")]
        public string Error;

        /// <summary>
        /// Error description
        /// </summary>
        [JsonProperty("error_description")]
        public string Description;

        /// <summary>
        /// Error uri
        /// </summary>
        [JsonProperty("error_uri")]
        public string Uri;
    }
}
