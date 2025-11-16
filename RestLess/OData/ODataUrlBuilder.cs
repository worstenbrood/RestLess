using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestLess.OData
{
    /// <summary>
    /// UrlBuilder with specific logic for odata
    /// </summary>
    public class ODataUrlBuilder<TClass> : UrlBuilder<ODataUrlBuilder<TClass>, 
        ODataQueryBuilder<TClass>>
    {
        private readonly ODataClient _client;
        private TClass[] _entries;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"></param>
        /// <param name="path"></param>
        public ODataUrlBuilder(ODataClient client, string path) : base(path, new ODataQueryBuilder<TClass>(path))
        {
            _client = client;
        }

        /// <summary>
        /// Add $select
        /// </summary>
        public ODataUrlBuilder<TClass> Select(params Expression<Select<TClass>>[] selectors)
        {
            QueryBuilder.Select(selectors);
            return this;
        }

        /// <summary>
        /// Add $expand
        /// </summary>
        public ODataUrlBuilder<TClass> Expand(params Expression<Expand<TClass>>[] expands)
        {
            QueryBuilder.Expand(expands);
            return this;
        }

        /// <summary>
        /// Add $expand
        /// </summary>
        public ODataUrlBuilder<TClass> Filter<TProperty>(Expression<Func<TClass, TProperty>> field, 
            Filter<TClass, TProperty> condition)
        {
            QueryBuilder.Filter(field, condition);
            return this;
        }

        /// <summary>
        /// Add " and " to the filter condition
        /// </summary>
        /// <returns></returns>
        public ODataUrlBuilder<TClass> And()
        {
            QueryBuilder.And();
            return this;
        }

        /// <summary>
        /// Add " or " to the filter condition
        /// </summary>
        /// <returns></returns>
        public ODataUrlBuilder<TClass> Or()
        {
            QueryBuilder.Or();
            return this;
        }

        /// <summary>
        /// Add $top
        /// </summary>
        public ODataUrlBuilder<TClass> Top(int count)
        {
            QueryBuilder.Top(count);
            return this;
        }

        /// <summary>
        /// Add $orderby (ascending)
        /// </summary>
        public ODataUrlBuilder<TClass> OrderBy(Expression<Select<TClass>> field)
        {
            QueryBuilder.OrderBy(field);
            return this;
        }

        /// <summary>
        /// Add $orderby (descending)
        /// </summary>
        public ODataUrlBuilder<TClass> OrderByDescending(Expression<Select<TClass>> field)
        {
            QueryBuilder.OrderByDescending(field);
            return this;
        }

        /// <summary>
        /// Set entries (used later by the Post calls)
        /// </summary>
        /// <param name="entries">Entries to set</param>
        public void Set(TClass[] entries)
        {
            _entries = entries;
        }

        /// <summary>
        /// Set a single entry (used later by the Post calls)
        /// </summary>
        /// <param name="entry">Entry to set</param>
        public void Set(TClass entry)
        {
            _entries = new[] { entry };
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            _entries = null;
            base.Reset();
        }  

        /// <summary>
        /// Reset query
        /// </summary>
        /// <returns></returns>
        public ODataUrlBuilder<TClass> ResetQuery()
        {
            Reset();
            return this;
        }

        /// <summary>
        /// Get entries from the api using this <see cref="ODataUrlBuilder{TClass}"/>
        /// </summary>
        /// <returns>An array of <typeparamref name="TClass"/></returns>
        public async Task<TClass[]> GetEntriesAsync()
        {
            return await _client.GetEntriesAsync(this);
        }

        /// <summary>
        /// Get entries from the api using this <see cref="ODataUrlBuilder{TClass}"/>
        /// </summary>
        /// <returns>An array of <typeparamref name="TClass"/></returns>
        public TClass[] GetEntries()
        {
            return _client.GetEntries(this);
        }

        /// <summary>
        /// Get a single entry from the api using this <see cref="ODataUrlBuilder{TClass}"/>
        /// </summary>
        /// <returns>A single <typeparamref name="TClass"/></returns>
        public Task<TClass> GetEntryAsync()
        {
            return _client.GetEntryAsync(this);
        }

        /// <summary>
        /// Get a single entry from the api using this <see cref="ODataUrlBuilder{TClass}"/>
        /// </summary>
        /// <returns>A single <typeparamref name="TClass"/></returns>
        public TClass GetEntry()
        {
            return _client.GetEntry(this);
        }     

        /// <summary>
        /// Post the entries set by the <see cref="Set(TClass)"/> and <see cref="Set(TClass[])"/> methods.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<ODataUrlBuilder<TClass>> PostEntriesAsync()
        {
            if (_entries == null || _entries.Length == 0)
            {
                throw new ArgumentNullException(nameof(_entries), "Use Set() first.");
            }

            await _client.PostEntriesAsync(this, _entries);
            return this;
        }

        /// <summary>
        /// Post the entries set by the <see cref="Set(TClass)"/> and <see cref="Set(TClass[])"/> methods.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public ODataUrlBuilder<TClass> PostEntries()
        {
            if (_entries == null || _entries.Length == 0)
            {
                throw new ArgumentNullException(nameof(_entries), "Use Set() first.");
            }

            _client.PostEntries(this, _entries);
            return this;
        }

        /// <summary>
        /// Post a single entry, no need to call Set() before
        /// </summary>
        /// <param name="entry"></param>
        public async Task<ODataUrlBuilder<TClass>> PostEntryAsync(TClass entry)
        {
            await _client.PostEntryAsync(this, entry);
            return this;
        }

        /// <summary>
        /// Post a single entry, no need to call Set() before
        /// </summary>
        /// <param name="entry"></param>
        public ODataUrlBuilder<TClass> PostEntry(TClass entry)
        {
            _client.PostEntry(this, entry);
            return this;
        }

        /// <summary>
        /// Delete entries specified by this <see cref="ODataUrlBuilder{TClass}"/>
        /// </summary>
        public async Task<ODataUrlBuilder<TClass>> DeleteEntriesAsync()
        {
            await _client.DeleteEntriesAsync(this);
            return this;
        }

        /// <summary>
        /// Delete entries specified by this <see cref="ODataUrlBuilder{TClass}"/>
        /// </summary>
        public ODataUrlBuilder<TClass> DeleteEntries()
        {
            _client.DeleteEntries(this);
            return this;
        }
    }
}
