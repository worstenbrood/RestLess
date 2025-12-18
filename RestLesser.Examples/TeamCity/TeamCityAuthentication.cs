using RestLesser.Authentication;
using System.Net.Http.Headers;

namespace RestLesser.Examples.TeamCity
{
    public class TeamCityAuthentication : IAuthentication
    {
        private readonly HttpClient _client;

        private readonly string _token;

        public TeamCityAuthentication(string url, string token)
        {
            _token = token;
            _client = new HttpClient
            {
                BaseAddress = new Uri(url),
                DefaultRequestHeaders = 
                {
                    Authorization = new AuthenticationHeaderValue("Basic", _token)
                }
            };
        }

        private string GetToken()
        {
            const string path = "authenticationTest.html?csrf";
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            using HttpResponseMessage response = _client.SendAsync(request).Sync();
            return response.Content.ReadAsStringAsync().Sync();
        }

        public void SetAuthentication(HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _token);
            request.Headers.Add("X-TC-CSRF-Token", GetToken());
        }
    }
}
