using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MazeSolver.Controllers.Tests
{
    [TestClass()]
    public class UnitTest1
    {
        [TestMethod()]
        [DeploymentItem("TestMaps")]
        public void PostTest()
        {
            var controller = new SolveMazeController();
            //var mazeText = System.IO.File.ReadAllText("maze1.txt");
            //var mazeText = System.IO.File.ReadAllText("maze2.txt");
            //var mazeText = System.IO.File.ReadAllText("maze3.txt");
            var mazeText = System.IO.File.ReadAllText("maze4.txt");
            var mazeAnswer = controller.Post(mazeText);
            var response = (MazeSolver.Controllers.JsonTextActionResult)mazeAnswer;
            //response.JsonText.Solution
            Console.WriteLine(response.JsonText.Steps.ToString());
            Console.WriteLine(response.JsonText.Solution);
            Assert.IsTrue(response.JsonText.Steps > 0);            
        }
    }
}