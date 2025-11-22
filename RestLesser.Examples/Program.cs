using RestLesser.Examples.TvMaze;

namespace RestLesser.Examples
{
    static class Program
    {
        public static void Main(params string[] args)
        {
            var tvMaze = new TvMazeClient();
            var results = tvMaze.SearchShow("Pluribus");
            foreach (var result in results)
            {
                Console.WriteLine($"Name: {result.Show?.Name}");
            }
        }
    }
}
