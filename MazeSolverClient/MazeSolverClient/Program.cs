using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MazeSolverClient
{
    class Program
    {
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("http://localhost:8080/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var answer = await SolveMazeAsync();
                Console.WriteLine($"Steps: {answer.Steps}");
                Console.WriteLine($"Solution:\n{answer.Solution}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        static async Task<MazeAnswer> SolveMazeAsync()
        {
            string maze = System.IO.File.ReadAllText(@"TestMaps\maze1.txt");
            //string maze = System.IO.File.ReadAllText(@"TestMaps\maze2.txt");
            //string maze = System.IO.File.ReadAllText(@"TestMaps\maze3.txt");
            //string maze = System.IO.File.ReadAllText(@"TestMaps\maze4.txt");
            HttpResponseMessage response = await client.PostAsJsonAsync("solveMaze", maze);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<MazeAnswer>();
        }
    }

    public class MazeAnswer
    {
        public int Steps { get; set; }
        public string Solution { get; set; }
    }
}
