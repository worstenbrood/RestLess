using System.IO;
using System.Text;
using RestLesser.DataAdapters;

namespace RestLesser.OAuth.Storage
{
    /// <summary>
    /// Token storage provider that loads and saves to local disk
    /// </summary>
    public class LocalStorage : ITokenStorage
    {
        private readonly JsonAdapter _adapter = new JsonAdapter();

        /// <inheritdoc/>
        public TokenData Load(string filename) => _adapter.Deserialize<TokenData>(File.ReadAllText(filename, Encoding.UTF8));
        
        /// <inheritdoc/>
        public void Save(string filename, TokenData token) => File.WriteAllText(filename, _adapter.Serialize(token), Encoding.UTF8);
    }
}
