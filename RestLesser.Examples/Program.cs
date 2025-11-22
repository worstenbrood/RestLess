using RestLesser.Examples.TvMaze;

namespace RestLesser.Examples
{
    static class Program
    {
        public static void Main(params string[] args)
        {
            var tvMaze = new TvMazeClient();
            var show = tvMaze.GetShow(1);
            Console.WriteLine($"Name: {show.Name}");
        }
    }
}
