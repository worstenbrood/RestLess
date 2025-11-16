using Newtonsoft.Json;

namespace RestLess.OAuth.Models
{
    public class ErrorResponse
    {
        [JsonProperty("error")]
        public string Error;

        [JsonProperty("error_description")]
        public string Description;

        [JsonProperty("error_uri")]
        public string Uri;
    }
}
