using RestLesser.OData.Interfaces;
using System.Threading.Tasks;

namespace RestLesser.OData
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TClass"></typeparam>
    /// <param name="client"></param>
    /// <param name="path"></param>
    public class ODataQuery<TClass>(IODataClient client, string path) : ODataUrlBuilder<TClass>(path)
    {
        /// <summary>
        /// Get entries from the api using this <see cref="ODataUrlBuilder{TClass}"/>
        /// </summary>
        /// <returns>An array of <typeparamref name="TClass"/></returns>
        public async Task<TClass[]> GetEntriesAsync()
        {
            return await client.GetEntriesAsync(this);
        }

        /// <summary>
        /// Get a single entry from the api using this <see cref="ODataUrlBuilder{TClass}"/>
        /// </summary>
        /// <returns>A single <typeparamref name="TClass"/></returns>
        public Task<TClass> GetEntryAsync()
        {
            return client.GetEntryAsync(this);
        }

        /*
        /// <summary>
        /// Put individual property
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<ODataQuery<TClass>> PutValueAsync<TProperty>(Expression<Func<TClass, TProperty>> field, TProperty value)
        {
            ValidateEntries();
            await client.PutValueAsync(this, _entries[0], field, value);
            return this;
        }
        */

        /*
        /// <summary>
        /// Put individual property
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ODataQuery<TClass> PutValue<TProperty>(Expression<Func<TClass, TProperty>> field, TProperty value)
        {
            ValidateEntries();
            client.PutValue(this, _entries[0], field, value);
            return this;
        }
        */
    }
}
