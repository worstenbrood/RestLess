using System;
using Newtonsoft.Json;
using RestLesser.OAuth.Models;

namespace RestLesser.OAuth.Storage
{
    /// <summary>
    /// Used to save/load token on disk
    /// </summary>
    
    public class TokenData
    {
        /// <summary>
        /// Token response
        /// </summary>
        [JsonProperty(PropertyName = "token_response")]
        public TokenResponse TokenResponse { get; private set; }

        /// <summary>
        /// Expiry date
        /// </summary>
        [JsonProperty(PropertyName = "expire_datetime")]
        public DateTime ExpireDateTime { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public TokenData()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TokenData(TokenResponse tokenResponse)
        {
            Update(tokenResponse);
        }

        /// <summary>
        /// Update token response
        /// </summary>
        /// <param name="tokenResponse"></param>
        public void Update(TokenResponse tokenResponse)
        {
            TokenResponse = tokenResponse;
            ExpireDateTime = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn);
        }
    }
}
