using RestLess.Authentication;
using RestLess.DataAdapters;
using System.Linq;

namespace RestLess.OData
{
    public class ODataClient : RestClient
    {
        public ODataClient(string baseUrl, IAuthentication authentication) : base(baseUrl, authentication, dataAdapter: new ODataJsonAdapter())
        { 
        }

        /// <summary>
        /// Generic method to fetch a collection of certain types and 
        /// be able to select which fields to select
        /// </summary>
        /// <typeparam name="T">Type to fetch</typeparam>
        /// <param name="builder">Url builder</param>
        /// <param name="selectors">Fields to select</param>
        /// <returns></returns>
        public T[] GetEntries<T>(ODataUrlBuilder<T> builder)
        {
            return Get<T[]>(builder.ToString());
        }

        /// <summary>
        /// Generic method to fetch a collection of certain types and 
        /// be able to select which fields to select
        /// </summary>
        /// <typeparam name="T">Type to fetch</typeparam>
        /// <param name="builder">Url builder</param>
        /// <param name="selectors">Fields to select</param>
        /// <returns></returns>
        public T GetEntry<T>(ODataUrlBuilder<T> builder)
        {
            return GetEntries(builder).FirstOrDefault();
        }

        /// <summary>
        /// Create entries (untested)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="entries"></param>
        public void PostEntries<T>(ODataUrlBuilder<T>  builder, T[] entries)
        {
            Post(builder.ToString(), entries);
        }

        /// <summary>
        /// Create a  single entry (untested)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="entries"></param>
        public void PostEntry<T>(ODataUrlBuilder<T> builder, T entry)
        {
            PostEntries(builder, new[] { entry });
        }

        /// Create entries (untested)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <param name="entries"></param>
        public void DeleteEntries<T>(ODataUrlBuilder<T> builder)
        {
            Delete(builder.ToString());
        }

        // Return query
        public ODataUrlBuilder<T> Query<T>(string url) => new ODataUrlBuilder<T>(this, url);
    }
}
