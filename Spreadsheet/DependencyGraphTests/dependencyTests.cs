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
            for (int i = 0; i < 1000; i++)
            {
                foreach(string parent in testKeys)
                {
                    graph.AddDependency(parent, rand.Next(0, 1000).ToString());
                }
            }
            foreach(string parent in testKeys)
            {
                Assert.IsTrue(graph.HasDependents(parent));
            }
        }

        [TestMethod]
        public void graphTest5()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            Random rand = new Random();
            for(int i = 0; i < 1000; i++)
            {
                foreach(string child in children)
                {
                    graph.AddDependency(i.ToString(), child);
                }
            }

            foreach(string child in children)
            {
                Assert.IsTrue(graph.HasDependees(child));
            }
        }

        [TestMethod]
        public void graphTest6()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] parent = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            
            foreach(string parents in parent)
            {
                foreach(string child in children)
                {
                    graph.AddDependency(parents, child);
                }
            }

            graph.RemoveDependency("a", "a");

            foreach(string aChildren in graph.GetDependents("a"))
            {
                Assert.AreNotEqual("a", aChildren);
            }
        }

        [TestMethod]
        public void graphTest7()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] parent = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };

            foreach (string parents in parent)
            {
                foreach (string child in children)
                {
                    graph.AddDependency(parents, child);
                }
            }

            foreach (string aChildren in graph.GetDependents("a"))
            {
                Assert.AreNotEqual("a", aChildren);
            }
        }
    }
}
