using System.IO;
using System.Text;
using RestLess.DataAdapters;

namespace RestLess.OAuth.Storage
{
    public class LocalStorage : ITokenStorage
    {
        private readonly JsonAdapter _adapter = new JsonAdapter();

        public TokenData Load(string filename) => _adapter.Deserialize<TokenData>(File.ReadAllText(filename, Encoding.UTF8));
        
        public void Save(string filename, TokenData token) => File.WriteAllText(filename, _adapter.Serialize(token), Encoding.UTF8);
    }
}
