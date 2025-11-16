using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using RestLesser.OAuth.Storage;
using RestLesser.Authentication;
using RestLesser.OAuth.Models;

namespace RestLesser.OAuth.Provider
{
    /// <summary>
    /// TokenProvider for Client Credentials flow
    /// </summary>
    public class ClientCredentials : TokenProvider
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Endpoint url</param>
        /// <param name="clientCertificate"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="storage"></param>
        public ClientCredentials(string endPoint, X509Certificate clientCertificate, string clientId, string clientSecret,
            ITokenStorage storage = null) : base(endPoint, clientCertificate, clientId, clientSecret, null, storage)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Endpoint url</param>
        /// <param name="authentication"></param>
        /// <param name="clientCertificate"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="storage"></param>
        public ClientCredentials(string endPoint, IAuthentication authentication, X509Certificate clientCertificate, 
            string clientId, string clientSecret, ITokenStorage storage = null) : 
                base(endPoint, authentication, clientCertificate, clientId, clientSecret, null, storage)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Endpoint url</param>
        /// <param name="authentication"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="storage"></param>
        public ClientCredentials(string endPoint, IAuthentication authentication, string clientId, 
            string clientSecret, ITokenStorage storage = null) : base(endPoint, authentication, clientId, 
                    clientSecret, null, storage)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint">Endpoint url</param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="storage"></param>
        public ClientCredentials(string endPoint, string clientId, string clientSecret, ITokenStorage storage = null) : 
            base(endPoint, clientId, clientSecret, null, storage)
        {
        }

        /// <summary>
        /// Get client credentials
        /// </summary>
        /// <returns></returns>
        public TokenResponse GetClientCredentials()
        {
            IEnumerable<KeyValuePair<string, string>> parameters = BaseParameters
                .Concat(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" }
                });

            return RestClient.Post<TokenResponse>(Uri.PathAndQuery, parameters);
        }

        /// <inheritdoc/>
        public override TokenResponse GetToken()
        {
            lock (this)
            {
                var fileName = $"{ClientId}.json";

                // Get token from disk
                TokenData tokenData = LoadToken();
                if (tokenData != null)
                {
                    if (tokenData.ExpireDateTime.AddMinutes(-1) >= DateTime.Now)
                    {
                        // Reuse stored token
                        return tokenData.TokenResponse;
                    }
                }
                else
                {
                    // New token
                    tokenData = new TokenData();
                }

                // Get new token and save
                tokenData.Update(GetClientCredentials());
                SaveToken(tokenData);

                return tokenData.TokenResponse;
            }
        }
    }
}
