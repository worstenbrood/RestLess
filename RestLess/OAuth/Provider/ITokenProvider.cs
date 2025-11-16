using RestLesser.OAuth.Models;

namespace RestLesser.OAuth.Provider
{
    /// <summary>
    /// Interface fro token providers
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Get token response
        /// </summary>
        /// <returns></returns>
        TokenResponse GetToken();
    }
}
