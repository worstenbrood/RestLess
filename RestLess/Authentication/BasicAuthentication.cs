using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RestLess.Authentication
{
    /// <summary>
    /// Authenticate using basic authentication
    /// </summary>
    public class BasicAuthentication : IAuthentication
    {
        private readonly string _username;
        private readonly string _password;

        public BasicAuthentication(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public void SetAuthentication(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_username}:{_password}")));
        }
    }
}
