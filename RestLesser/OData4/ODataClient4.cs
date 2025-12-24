using RestLesser.Authentication;
using RestLesser.OData;
using RestLesser.OData.Interfaces;
using RestLesser.OData4.Models;
using System.Threading.Tasks;

namespace RestLesser.OData4
{
    /// <summary>
    /// OData4 
    /// </summary>
    /// <param name="baseUrl"></param>
    /// <param name="authentication"></param>
    public class ODataClient4(string baseUrl, IAuthentication authentication) : RestClient(baseUrl, authentication), IODataClient
    {
        /// <inheritdoc/>
        public TClass GetEntry<TClass>(ODataUrlBuilder<TClass> builder)
        {
            return Get<TClass>(builder.ToString());
        }

        /// <inheritdoc/>
        public async Task<TClass> GetEntryAsync<TClass>(ODataUrlBuilder<TClass> builder)
        {
            return await GetAsync<TClass>(builder.ToString());
        }

        /// <inheritdoc/>
        public void PostEntry<T>(ODataUrlBuilder<T> builder, T entry)
        {
            Post(builder.ToString(), entry);
        }

        /// <inheritdoc/>
        public async Task PostEntryAsync<T>(ODataUrlBuilder<T> builder, T entry)
        {
            await PostAsync(builder.ToString(), entry);
        }

        /// <inheritdoc/>
        public TClass[] GetEntries<TClass>(ODataUrlBuilder<TClass> builder)
        {
            var result = Get<Result4<TClass>>(builder.ToString());
            return result.Value;
        }

        /// <inheritdoc/>
        public async Task<TClass[]> GetEntriesAsync<TClass>(ODataUrlBuilder<TClass> builder)
        {
            var result = await GetAsync<Result4<TClass>>(builder.ToString());
            return result.Value;
        }
        
        /// <inheritdoc/>
        public void PostEntries<T>(ODataUrlBuilder<T> builder, T[] entries)
        {
            Post(builder.ToString(), entries);
        }

        /// <inheritdoc/>
        public async Task PostEntriesAsync<T>(ODataUrlBuilder<T> builder, T[] entries)
        {
            await PostAsync(builder.ToString(), entries);
        }

        /// <inheritdoc/>
        public void DeleteEntries<T>(ODataUrlBuilder<T> builder)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public Task DeleteEntriesAsync<T>(ODataUrlBuilder<T> builder)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public ODataUrlBuilder<T> Query<T>(string url) => new (this, url);
    }
}
