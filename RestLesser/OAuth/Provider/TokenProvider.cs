using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using RestLesser.Authentication;
using RestLesser.OAuth.Storage;
using RestLesser.OAuth.Models;

namespace RestLesser.OAuth.Provider
{
    /// <summary>
    /// TokenProvider for Authorization/Refresh flow
    /// </summary>
    public class TokenProvider : ITokenProvider
    {
        /// <summary>
        /// Endpoint uri
        /// </summary>
        protected readonly Uri Uri;

        /// <summary>
        /// Client ID
        /// </summary>
        protected readonly string ClientId;

        /// <summary>
        /// Post parameters
        /// </summary>
        protected readonly Dictionary<string, string> BaseParameters = new ();

        /// <summary>
        /// Rest client
        /// </summary>
        protected readonly OAuthClient RestClient;

        /// <summary>
        /// Token storage implementation
        /// </summary>
        protected readonly ITokenStorage TokenStorage;

        /// <summary>
        /// Token storage filename
        /// </summary>
        protected readonly string Filename;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="clientId"></param>
        /// <param name="tokenStorage"></param>
        private TokenProvider(string endPoint, string clientId, ITokenStorage tokenStorage)
        {
            Uri = new Uri(endPoint);
            TokenStorage = tokenStorage ?? new LocalStorage();           
            ClientId = clientId;
            Filename = $"{ClientId}.json";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="clientCertificate"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUri"></param>
        /// <param name="storage"></param>
        public TokenProvider(string endPoint, X509Certificate clientCertificate, string clientId, string clientSecret, 
            string redirectUri, ITokenStorage storage = null) : this(endPoint, clientId, storage) 
        {
            RestClient = new OAuthClient(endPoint, clientCertificate, null);
            BaseParameters
                .TryAddValue("client_id", clientId)
                .TryAddValue("client_secret", clientSecret)
                .TryAddValue("redirect_uri", redirectUri);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="authentication"></param>
        /// <param name="clientCertificate"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUri"></param>
        /// <param name="storage"></param>
        public TokenProvider(string endPoint, IAuthentication authentication, X509Certificate clientCertificate, 
            string clientId, string clientSecret, string redirectUri, ITokenStorage storage = null) : 
                this(endPoint, clientId, storage) 
        {
            RestClient = new OAuthClient(endPoint, clientCertificate, authentication);
            BaseParameters
                .TryAddValue("client_id", clientId)
                .TryAddValue("client_secret", clientSecret)
                .TryAddValue("redirect_uri", redirectUri);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="authentication"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUri"></param>
        /// <param name="storage"></param>
        public TokenProvider(string endPoint, IAuthentication authentication, string clientId, string clientSecret, 
            string redirectUri, ITokenStorage storage = null) : this(endPoint, clientId, storage)
        {
            RestClient = new OAuthClient(endPoint, authentication, null);
            BaseParameters
                .TryAddValue("client_id", clientId)
                .TryAddValue("client_secret", clientSecret)
                .TryAddValue("redirect_uri", redirectUri);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUri"></param>
        /// <param name="storage"></param>
        public TokenProvider(string endPoint, string clientId, string clientSecret, string redirectUri, 
            ITokenStorage storage = null) : this(endPoint, clientId, storage)
        {
            RestClient = new OAuthClient(endPoint);
            BaseParameters
                .TryAddValue("client_id", clientId)
                .TryAddValue("client_secret", clientSecret)
                .TryAddValue("redirect_uri", redirectUri);
        }

        /// <summary>
        /// Load token from storage
        /// </summary>
        /// <returns></returns>
        protected TokenData LoadToken() => TokenStorage.Load(Filename);

        /// <summary>
        /// Save token to storage
        /// </summary>
        /// <param name="data"></param>
        protected void SaveToken(TokenData data) => TokenStorage.Save(Filename, data);

        /// <summary>
        /// Get access token using <paramref name="authorizationCode"/>
        /// </summary>
        /// <param name="authorizationCode">Authorization code</param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieves a new access token using the refresh token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
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
        
        /// <inheritdoc/>
        public virtual TokenResponse GetToken()
        {
            lock (this)
            {
                // Get token from disk, previously stored from GetAccessToken
                TokenData tokenData = LoadToken() ?? throw new Exception($"No token found for {ClientId}");

                // Check if token is expired
                if (!tokenData.IsExpired)
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