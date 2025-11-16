namespace RestLess.OAuth.Storage
{
    public interface ITokenStorage
    {
        TokenData Load(string filename);
        void Save(string filename, TokenData token);
    }
}
