using RestLesser.Examples.TvMaze.Models;
using System.Web;

namespace RestLesser.Examples.TvMaze
{
    public class TvMazeClient : RestClient
    {
        public TvMazeClient() : base("https://api.tvmaze.com")
        { 
        }

        public ShowSearchResult[] SearchShow(string query)
        {
            return Get<ShowSearchResult[]>($"/search/shows?q={HttpUtility.UrlEncode(query)}");
        }

        public Show GetShow(int id)
        {
            return Get<Show>($"/shows/{id}");
        }
    }
}
