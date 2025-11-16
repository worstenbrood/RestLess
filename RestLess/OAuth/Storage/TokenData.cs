using System;
using Newtonsoft.Json;
using RestLess.OAuth.Models;

namespace RestLess.OAuth.Storage
{
    /// <summary>
    /// Used to save/load token on disk
    /// </summary>
    
    public class TokenData
    {
        [JsonProperty(PropertyName = "token_response")]
        public TokenResponse TokenResponse { get; private set; }

        [JsonProperty(PropertyName = "expire_datetime")]
        public DateTime ExpireDateTime { get; private set; }

        public TokenData()
        {
        }

        public TokenData(TokenResponse tokenResponse)
        {
            Update(tokenResponse);
        }

        public void Update(TokenResponse tokenResponse)
        {
            TokenResponse = tokenResponse;
            ExpireDateTime = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn);
        }
    }
}
