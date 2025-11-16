using System.Linq;
using System.Threading.Tasks;
using RestLesser.Authentication;
using RestLesser.DataAdapters;

namespace RestLesser.OData
{
    /// <summary>
    /// OData rest client
    /// </summary>
    public class ODataClient : RestClient
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="authentication"></param>
        public ODataClient(string baseUrl, IAuthentication authentication) : base(baseUrl, authentication, dataAdapter: new ODataJsonAdapter())
        { 
        }

        /// <summary>
        /// Generic method to fetch a collection of certain types and 
        /// be able to select which fields to select
        /// </summary>
        /// <typeparam name="TClass">Type to fetch</typeparam>
        /// <param name="builder">Url builder</param>
        /// <returns></returns>
        public async Task<TClass[]> GetEntriesAsync<TClass>(ODataUrlBuilder<TClass> builder)
        {
            return await GetAsync<TClass[]>(builder.ToString());
        }

        /// <summary>
        /// GetEntries sync
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public TClass[] GetEntries<TClass>(ODataUrlBuilder<TClass> builder)
        {
            return Get<TClass[]>(builder.ToString());
        }

        /// <summary>
        /// Generic method to fetch a collection of certain types and 
        /// be able to select which fields to select
        /// </summary>
        /// <typeparam name="TClass">Type to fetch</typeparam>
        /// <param name="builder">Url builder</param>
        /// <returns></returns>
        public async Task<TClass> GetEntryAsync<TClass>(ODataUrlBuilder<TClass> builder)
        {
            return (await GetEntriesAsync(builder)).FirstOrDefault();
        }

        /// <summary>
        /// GetEntry sync
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public TClass GetEntry<TClass>(ODataUrlBuilder<TClass> builder)
        {
            return GetEntries(builder).FirstOrDefault();
        }

        /// <summary>
        /// Create entries (untested)
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="builder"></param>
        /// <param name="entries"></param>
        public async Task PostEntriesAsync<TClass>(ODataUrlBuilder<TClass> builder, TClass[] entries)
        {
            await PostAsync(builder.ToString(), entries);
        }

        /// <summary>
        /// PostEntries sync
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="builder"></param>
        /// <param name="entries"></param>
        public void PostEntries<TClass>(ODataUrlBuilder<TClass> builder, TClass[] entries)
        {
            Post(builder.ToString(), entries);
        }

        /// <summary>
        /// Create a  single entry (untested)
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="builder"></param>
        /// <param name="entry"></param>
        public async Task PostEntryAsync<TClass>(ODataUrlBuilder<TClass> builder, TClass entry)
        {
            await PostEntriesAsync(builder, new[] { entry });
        }

        /// <summary>
        /// PostEntry sync
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="builder"></param>
        /// <param name="entry"></param>
        public void PostEntry<TClass>(ODataUrlBuilder<TClass> builder, TClass entry)
        {
            PostEntries(builder, new[] { entry });
        }

        /// <summary>
        /// Create entries (untested)
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="builder"></param>
        public async Task DeleteEntriesAsync<TClass>(ODataUrlBuilder<TClass> builder)
        {
            await DeleteAsync(builder.ToString());
        }

        /// <summary>
        /// DeleteEntries sync
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="builder"></param>
        public void DeleteEntries<TClass>(ODataUrlBuilder<TClass> builder)
        {
            Delete(builder.ToString());
        }

        /// <summary>
        /// Create a <see cref="ODataUrlBuilder{TClass}"/> using this client
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public ODataUrlBuilder<TClass> Query<TClass>(string url) => new ODataUrlBuilder<TClass>(this, url);
    }
}
