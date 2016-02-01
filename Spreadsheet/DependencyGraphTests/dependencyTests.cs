using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
namespace DependencyGraphTests
{
    [TestClass]
    public class dependencyTests
    {
        [TestMethod]
        public void graphTest1()
        {
            DependencyGraph graph = new DependencyGraph();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void graphTest2()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.HasDependees(null);
        }
        [TestMethod]
        public void graphTest3()
        {
            DependencyGraph graph = new DependencyGraph();
            Assert.AreEqual(graph.Size, 0);
        }

        [TestMethod]
        public void graphTest4()
        {
            DependencyGraph graph = new DependencyGraph();

            Random rand = new Random();
            string[] testKeys = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            int num = 1;
            int increaseNum = 10;
            for (int i = 0; i < 10000; i++)
            {
                increaseNum += num/2;
                graph.AddDependency(testKeys[rand.Next(0, 10)], rand.Next(0,1000000000).ToString());
                num = increaseNum;
            }
            foreach(string parent in testKeys)
            {
                Assert.IsTrue(graph.HasDependents(parent));
            }



        }
    }
}
