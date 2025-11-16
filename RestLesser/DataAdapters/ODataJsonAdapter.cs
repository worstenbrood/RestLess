using RestLesser.OData.Models;

namespace RestLesser.DataAdapters
{
    /// <summary>
    /// OData data adapter
    /// </summary>
    public class ODataJsonAdapter : JsonAdapter
    {
        /// <inheritdoc/>
        public override T Deserialize<T>(string body)
        {
            var result = base.Deserialize<Result<T>>(body);
            
            return result.Data.Results;
        }

        /// <inheritdoc/>
        public override string Serialize<T>(T data)
        {
            var result = new Result<T>
            {
                Data = new Data<T>
                {
                    Results = data
                }
            };
            
            return base.Serialize(result);
        }
    }
}
