namespace RestLesser.OAuth.Storage
{
    /// <summary>
    /// Interface for token storage providers
    /// </summary>
    public interface ITokenStorage
    {
        /// <summary>
        /// Load token
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        TokenData Load(string filename);

        /// <summary>
        /// Save token
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="token"></param>
        void Save(string filename, TokenData token);
    }
}
