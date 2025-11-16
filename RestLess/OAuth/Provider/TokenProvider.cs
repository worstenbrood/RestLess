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
    /// TokenProvider for Authorization/Refresh flow
    /// </summary>
    public class TokenProvider : ITokenProvider
    {
        protected readonly Uri Uri;
        protected readonly string ClientId;
        protected readonly Dictionary<string, string> BaseParameters = new Dictionary<string, string>();
        protected readonly OAuthClient RestClient;
        protected readonly ITokenStorage TokenStorage;
        protected readonly string Filename;

        private TokenProvider(string endPoint, string clientId, ITokenStorage tokenStorage)
        {
            Uri = new Uri(endPoint);
            TokenStorage = tokenStorage ?? new LocalStorage();           
            ClientId = clientId;
            Filename = $"{ClientId}.json";
        }

        public TokenProvider(string endPoint, X509Certificate clientCertificate, string clientId, string clientSecret, 
            string redirectUri, ITokenStorage storage = null) : this(endPoint, clientId, storage) 
        {
            RestClient = new OAuthClient(endPoint, clientCertificate, null);
            BaseParameters
                .AddIfNotEmpty("client_id", clientId)
                .AddIfNotEmpty("client_secret", clientSecret)
                .AddIfNotEmpty("redirect_uri", redirectUri);
        }

        public TokenProvider(string endPoint, IAuthentication authentication, X509Certificate clientCertificate, 
            string clientId, string clientSecret, string redirectUri, ITokenStorage storage = null) : 
                this(endPoint, clientId, storage) 
        {
            RestClient = new OAuthClient(endPoint, clientCertificate, authentication);
            BaseParameters
                .AddIfNotEmpty("client_id", clientId)
                .AddIfNotEmpty("client_secret", clientSecret)
                .AddIfNotEmpty("redirect_uri", redirectUri);
        }

        public TokenProvider(string endPoint, IAuthentication authentication, string clientId, string clientSecret, 
            string redirectUri, ITokenStorage storage = null) : this(endPoint, clientId, storage)
        {
            RestClient = new OAuthClient(endPoint, authentication, null);
            BaseParameters
                .AddIfNotEmpty("client_id", clientId)
                .AddIfNotEmpty("client_secret", clientSecret)
                .AddIfNotEmpty("redirect_uri", redirectUri);
        }

        public TokenProvider(string endPoint, string clientId, string clientSecret, string redirectUri, 
            ITokenStorage storage = null) : this(endPoint, clientId, storage)
        {
            RestClient = new OAuthClient(endPoint);
            BaseParameters
                .AddIfNotEmpty("client_id", clientId)
                .AddIfNotEmpty("client_secret", clientSecret)
                .AddIfNotEmpty("redirect_uri", redirectUri);
        }

        protected TokenData LoadToken() => TokenStorage.Load(Filename);
        protected void SaveToken(TokenData data) => TokenStorage.Save(Filename, data);

        public TokenResponse GetAccessToken(string authorizationCode)
        {
            IEnumerable<KeyValuePair<string, string>> parameters = BaseParameters
                .Concat(new Dictionary<string, string>
                {
                    { "code", authorizationCode },
                    { "grant_type", "authorization_code" }
                });

            return RestClient.Post<TokenResponse>(Uri.PathAndQuery, parameters);
        }

        /// <summary>
        /// Retrieves an access token and save it
        /// </summary>
        /// <param name="accessToken"></param>
        public void SaveAccessToken(string accessToken)
        {
            var tokenResponse = GetAccessToken(accessToken);
            var tokenData = new TokenData(tokenResponse);
            SaveToken(tokenData);
        }
        
        public TokenResponse GetRefreshToken(string refreshToken)
        {
            IEnumerable<KeyValuePair<string, string>> parameters = BaseParameters
                .Concat(new Dictionary<string, string>
                {
                    { "refresh_token", refreshToken },
                    { "grant_type", "refresh_token" }
                });

            return RestClient.Post<TokenResponse>(Uri.PathAndQuery, parameters);
        }
        
        public virtual TokenResponse GetToken()
        {
            lock (this)
            {
                // Get token from disk, previously stored from GetAccessToken
                TokenData tokenData = LoadToken() ?? throw new Exception($"No token found for {ClientId}");

                // Check if token is expired
                if (tokenData.ExpireDateTime.AddMinutes(-1) >= DateTime.Now)
                {
                    return tokenData.TokenResponse;
                }

                // Refresh token
                var tokenResponse = GetRefreshToken(tokenData.TokenResponse.RefreshToken);
                tokenData.Update(tokenResponse);

                // Save
                SaveToken(tokenData);
                return tokenData.TokenResponse;
            }
        }
    }
}