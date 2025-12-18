using Newtonsoft.Json;
using RestLesser.DataAdapters;
using RestLesser.Examples.TvMaze;

namespace RestLesser.Examples
{
    static class Program
    {
        public static void Main(params string[] args)
        {
            var json = new JsonAdapter();
            // Pretty print
            json.SerializerSettings.Formatting = Formatting.Indented;

            var tvMaze = new TvMazeClient();
            var results = tvMaze.SearchShow("Walking Dead");
            Console.WriteLine(json.Serialize(results));
        }
    }
}
