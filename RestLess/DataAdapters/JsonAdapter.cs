using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace RestLess.DataAdapters
{
    /// <summary>
    /// DataAdapter using NewtonSoft.Json
    /// </summary>
    public class JsonAdapter : IDataAdapter
    {        
        private JsonSerializerSettings _serializerSettings;

        /// <summary>
        /// Serializer settings
        /// </summary>
        public virtual JsonSerializerSettings SerializerSettings => _serializerSettings ??= new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
        };

        /// <inheritdoc/>
        public MediaTypeWithQualityHeaderValue MediaTypeHeader { get; } = new MediaTypeWithQualityHeaderValue("application/json");

        /// <inheritdoc/>
        public virtual string Serialize<T>(T data)
        {
            return JsonConvert.SerializeObject(data, SerializerSettings);
        }

        /// <inheritdoc/>
        public virtual T Deserialize<T>(string body)
        {
            return (T)JsonConvert.DeserializeObject(body, typeof(T), SerializerSettings);
        }
    }
}
