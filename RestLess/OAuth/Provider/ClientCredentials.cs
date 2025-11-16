using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using RestLess.OAuth.Storage;
using RestLess.OAuth.Models;
using RestLess.Authentication;

namespace RestLess.OAuth.Provider
{

    /// <summary>
    /// TokenProvider for Client Credentials flow
    /// </summary>
    public class ClientCredentials : TokenProvider
    {
        public ClientCredentials(string endPoint, X509Certificate clientCertificate, string clientId, string clientSecret,
            ITokenStorage storage = null) : base(endPoint, clientCertificate, clientId, clientSecret, null, storage)
        {
        }

        public ClientCredentials(string endPoint, IAuthentication authentication, X509Certificate clientCertificate, 
            string clientId, string clientSecret, ITokenStorage storage = null) : 
                base(endPoint, authentication, clientCertificate, clientId, clientSecret, null, storage)
        {
        }

        public ClientCredentials(string endPoint, IAuthentication authentication, string clientId, 
            string clientSecret, ITokenStorage storage = null) : base(endPoint, authentication, clientId, 
                    clientSecret, null, storage)
        {
        }

        public ClientCredentials(string endPoint, string clientId, string clientSecret, ITokenStorage storage = null) : 
            base(endPoint, clientId, clientSecret, null, storage)
        {
        }

        public TokenResponse GetClientCredentials()
        {
            IEnumerable<KeyValuePair<string, string>> parameters = BaseParameters
                .Concat(new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" }
                });

            return RestClient.Post<TokenResponse>(Uri.PathAndQuery, parameters);
        }

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
