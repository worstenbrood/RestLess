using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace RestLesser.DataAdapters
{
    /// <summary>
    /// SnakeCase Json data adapter
    /// </summary>
    public class SnakeCaseJsonAdapter : JsonAdapter
    {
        private JsonSerializerSettings _serializerSettings;

        /// <summary>
        /// Serializer settings
        /// </summary>
        public override JsonSerializerSettings SerializerSettings =>
            _serializerSettings ??= new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy
                    {
                        ProcessDictionaryKeys = true,
                    }
                }
            };
    }
}
