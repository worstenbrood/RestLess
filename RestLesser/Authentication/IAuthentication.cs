using System.Net.Http;

namespace RestLesser.Authentication
{
    /// <summary>
    /// Interface for authentication methods
    /// </summary>
    public interface IAuthentication
    {
        /// <summary>
        /// Set authentication on request message
        /// </summary>
        /// <param name="request"></param>
        void SetAuthentication(HttpRequestMessage request);
    }
}
