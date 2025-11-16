using System.Net.Http;

namespace RestLesser.Authentication
{
    /// <summary>
    /// Authenticate using basic authentication
    /// </summary>
    public class BasicAuthentication : IAuthentication
    {
        private readonly string _username;
        private readonly string _password;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public BasicAuthentication(string username, string password)
        {
            _username = username;
            _password = password;
        }

        /// <inheritdoc/>
        public void SetAuthentication(HttpRequestMessage request)
        {
            request.SetBasic(_username, _password);
        }
    }
}
