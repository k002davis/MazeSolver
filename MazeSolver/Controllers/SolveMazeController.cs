using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace MazeSolver.Controllers
{
    public class SolveMazeController : ApiController
    {        
        protected internal virtual JsonTextActionResult JsonText(MazeAnswer jsonText)
        {
            return new JsonTextActionResult(Request, jsonText);
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] string map)
        {
            var stringList = GetMazeStringList(map);
            string[] newMap = stringList.ToArray();
            bool pathFound = false;

            // locate A and B points
            int aX = -1;
            int aY = -1;
            int bX = -1;
            int bY = -1;
            for (int i = 0; i < newMap.Length; i++)
            {
                string oneRow = newMap[i];
                if (oneRow.Contains("A"))
                {
                    for (int y = 0; y < oneRow.Length; y++)
                    {
                        if (oneRow[y] == 'A')
                        {
                            aX = y;
                            aY = i;
                            break;
                        }
                    }
                }
                else if (oneRow.Contains("B"))
                {
                    for (int y = 0; y < oneRow.Length; y++)
                    {
                        if (oneRow[y] == 'B')
                        {
                            bX = y;
                            bY = i;
                            break;
                        }
                    }
                }
            }

            Location current = null;
            var start = new Location { X = aX, Y = aY };
            var target = new Location { X = bX, Y = bY };
            var openList = new List<Location>();
            var closedList = new List<Location>();
            int g = 0;
            int steps = 0;

            // start by adding the original position to the open list
            openList.Add(start);

            while (openList.Count > 0)
            {
                // get the square with the lowest F score
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                // add the current square to the closed list
                closedList.Add(current);

                // remove it from the open list
                openList.Remove(current);

                // if we added the destination to the closed list, we've found a path
                if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null)
                    break;

                var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, newMap);
                g++;

                foreach (var adjacentSquare in adjacentSquares)
                {
                    // if this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) != null)
                        continue;

                    // if it's not in the open list...
                    if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) == null)
                    {
                        // compute its score, set the parent
                        adjacentSquare.G = g;
                        adjacentSquare.H = ComputeHScore(adjacentSquare.X, adjacentSquare.Y, target.X, target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // and add it to the open list
                        openList.Insert(0, adjacentSquare);
                    }
                    else
                    {
                        // test if using the current G score makes the adjacent square's F score
                        // lower, if yes update the parent because it means it's a better path
                        if (g + adjacentSquare.H < adjacentSquare.F)
                        {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }
            }

            // assume path was found; update map
            while (current != null)
            {
                if (current.X == aX && current.Y == aY)
                {
                    steps += 1;
                }
                else if (current.X == bX && current.Y == bY)
                {
                    // do nothing
                }
                else
                {
                    steps += 1;
                    char[] array = stringList[current.Y].ToCharArray();
                    array[current.X] = '@';
                    stringList[current.Y] = new string(array);
                    pathFound = true;
                }
                current = current.Parent;
            }

            List<string> outMap = new List<string>();

            if (pathFound)
            {
                for (int i = 0; i < stringList.Count; i++)
                {
                    if (i != 0 && i != stringList.Count - 1)
                    {
                        outMap.Add(stringList[i].Substring(1, stringList[i].Length - 2));
                    }
                }
            }
            else
            {
                steps = 0;
            }
            
            var answer = new MazeAnswer
            {
                Steps = steps,
                Solution = BuildFinalMapString(pathFound, outMap)
            };
            
            return JsonText(answer);
        }

        private string BuildFinalMapString(bool pathFound, List<string> inMap)
        {
            if (pathFound)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                // Append to StringBuilder
                foreach (var outLine in inMap)
                {
                    builder.Append(outLine).AppendLine();
                }
                // Clean up last \r\n
                builder.Length -= 2;
                return builder.ToString();
            }
            else
            {
                return "unable to locate solution to maze";
            }
        }

        private List<string> GetMazeStringList(string maze)
        {
            string[] lines = maze.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            // Add border around entire map else proposedLocations could throw index exception
            int rowLength = lines[0].Length;
            string border = "";
            for (int i = 0; i < rowLength + 2; i++)
            {
                border += "#";
            }

            List<string> myCollection = new List<string>
            {
                border
            };
            foreach (var oneLine in lines)
            {
                myCollection.Add("#" + oneLine + "#");
            }
            myCollection.Add(border);
            return myCollection;
        }

        private List<Location> GetWalkableAdjacentSquares(int x, int y, string[] map)
        {
            var proposedLocations = new List<Location>()
            {
                new Location { X = x, Y = y - 1 },
                new Location { X = x, Y = y + 1 },
                new Location { X = x - 1, Y = y },
                new Location { X = x + 1, Y = y },
            };

            return proposedLocations.Where(l => map[l.Y][l.X] == '.' || map[l.Y][l.X] == 'B').ToList();
        }

        private int ComputeHScore(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }

    }

    internal class Location
    {
        public int X;
        public int Y;
        public int F;
        public int G;
        public int H;
        public Location Parent;
    }

    public class MazeAnswer
    {
        public int Steps { get; set; }
        public string Solution { get; set; }
    }

    public class JsonTextActionResult : IHttpActionResult
    {
        public HttpRequestMessage Request { get; }
        public MazeAnswer JsonText { get; }

        public JsonTextActionResult(HttpRequestMessage request, MazeAnswer jsonText)
        {
            Request = request;
            JsonText = jsonText;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        public HttpResponseMessage Execute()
        {
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new ObjectContent<MazeAnswer>(JsonText, new System.Net.Http.Formatting.JsonMediaTypeFormatter());

            return response;
        }
    }
}
