# MazeSolver

C# Web API Maze Solver and Client

Web API Service accepts POST with "map" as string in body. Endpoint is /solveMaze. Returns JSON response that contains integer property 'steps' and string property 'solution'. Solution contains one Unit Test with test maps embedded. Included in repository is Maze Solver Client console application.

How to test/run Web API Service w/Fiddler: Rebuild entire solution (MazeSolver). Start debugging Web API. Using Fiddler -> Composer, Set action to POST, url = http://localhost:8080/solveMaze Copy test map text into "Request Body" enclosing with double quotes, click EXECUTE

How to test Web API Service w/Unit Test: Open Controllers/UnitTest1.cs Right click on PostTest() method name and select "Debug Tests"

How to test Web API Service w/Client: Rebuild entire solution (MazeSolver). Start debugging Web API. Open MazeSolverClient solution with new instance of Visual Studio Debug MazeSolverClient.

A* path finding algorithm and code snippets from http://gigi.nullneuron.net/gigilabs/a-pathfinding-example-in-c/.
