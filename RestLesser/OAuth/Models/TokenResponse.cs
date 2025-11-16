using Newtonsoft.Json;

namespace RestLesser.OAuth.Models
{
    /// <summary>
    /// OAuth token response
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// The access token string as issued by the authorization server.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken;

        /// <summary>
        /// The type of token this is, typically just the string “Bearer”.
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType;

        /// <summary>
        /// If the access token expires, the server should reply with the duration of time the access token is granted for.
        /// </summary>
        [JsonProperty("expires_in")]
        public int ExpiresIn;

        /// <summary>
        /// If the access token will expire, then it is useful to return a refresh token which applications can use to obtain
        /// another access token. However, tokens issued with the implicit grant cannot be issued a refresh token.
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken;

        /// <summary>
        /// If the scope the user granted is identical to the scope the app requested, this parameter is optional. If the granted
        /// scope is different from the requested scope, such as if the user modified the scope, then this parameter is required.
        /// </summary>
        [JsonProperty("scope")]
        public string Scope;
    }
}
