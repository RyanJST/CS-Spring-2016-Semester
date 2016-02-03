using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;

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
            string[] testChildren = new string[10];
            int i = 0;
            foreach (string parents in parent)
            {
                foreach (string child in children)
                {
                    graph.AddDependency(parents, child);
                }
            }


            foreach (string aChildren in graph.GetDependents("a"))
            {
                testChildren[i] = aChildren;
                i++;
            }
            for (int j = 0; j < children.Length; j++){
                Assert.AreEqual(children[j], testChildren[j]);
            }
        }

        [TestMethod]
        public void graphTest8()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] parent = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] testChildren = new string[10];
            int i = 0;
            foreach (string parents in parent)
            {
                foreach (string child in children)
                {
                    graph.AddDependency(parents, child);
                }
            }

            foreach (string aParents in graph.GetDependees("a"))
            {
                testChildren[i] = aParents;
                i++;
            }
            for (int j = 0; j < parent.Length; j++)
            {
                Assert.AreEqual(parent[j], testChildren[j]);
            }
        }

        [TestMethod]
        public void graphTest9()
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
            Assert.AreEqual(100, graph.Size);
        }

        [TestMethod]
        public void graphTest10()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] newChildren = new string[10] {"k", "l", "m", "n", "o", "p", "q", "r", "s", "t"};
            string[] parent = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };

            string[] testChild = new string[10];

            int i = 0;

            foreach (string parents in parent)
            {
                foreach (string child in children)
                {
                    graph.AddDependency(parents, child);
                }
            }

            graph.ReplaceDependents("b", newChildren);

            foreach(string child in graph.GetDependents("b"))
            {
                testChild[i] = child;
                i++;
            }

            for(int j = 0; j < newChildren.Length; j++)
            {
                Assert.AreEqual(newChildren[j], testChild[j]);
            }
        }


        [TestMethod]
        public void graphTest11()
        {
            DependencyGraph graph = new DependencyGraph();
            
            for(int i = 0; i< 10000; i++)
            {
                graph.AddDependency("a", "b");
                graph.RemoveDependency("a", "b");
            }
        }

        [TestMethod]
        public void graphTest12()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] newChildren = new string[10] { "k", "l", "m", "n", "o", "p", "q", "r", "s", "t" };
            string[] parent = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };

            string[] testChild = new string[10];

            int i = 0;

            foreach (string parents in parent)
            {
                foreach (string child in children)
                {
                    graph.AddDependency(parents, child);
                }
            }

            graph.ReplaceDependees("b", newChildren);

            foreach (string newParent in graph.GetDependees("b"))
            {
                testChild[i] = newParent;
                i++;
            }

            for (int j = 0; j < newChildren.Length; j++)
            {
                Assert.AreEqual(newChildren[j], testChild[j]);
            }
        }

        [TestMethod]
        public void graphTest13()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] newChildren = new string[10] { "k", "l", "m", "n", "o", "p", "q", "r", "s", "t" };
            
            foreach(string child in children)
            {
                foreach(string parent in newChildren)
                {
                    graph.AddDependency(parent, child);
                }
            }

            foreach (string child in children)
            {
                graph.RemoveDependency("m", child);
            }

            Assert.IsFalse(graph.HasDependees("m"));
        }

        [TestMethod]
        public void graphTest14()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] newChildren = new string[10] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a" };

            foreach (string child in children)
            {
                foreach (string parent in newChildren)
                {
                    graph.AddDependency(parent, child);
                }
            }

            Assert.AreEqual(10,graph.Size);
        }

        [TestMethod]
        public void graphTest15()
        {
            DependencyGraph graph = new DependencyGraph();

            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");

            List<string> tempList = new List<string>();

            foreach(string child in graph.GetDependents("a"))
            {
                tempList.Add(child);
            }

            tempList.Add("d");

            Assert.IsTrue(new HashSet<string> { "b", "c", "d" }.SetEquals(tempList));
            Assert.IsTrue(new HashSet<string> { "b", "c" }.SetEquals(graph.GetDependents("a")));
        }

        [TestMethod]
        public void graphTest16()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            Random rand = new Random();
            for (int i = 0; i < 10000; i++)
            {
                foreach (string child in children)
                {
                    graph.AddDependency(i.ToString(), child);
                }
            }

            foreach (string child in children)
            {
                Assert.IsTrue(graph.HasDependees(child));
            }
        }

        [TestMethod]
        public void graphTest17()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] parent = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            Random rand = new Random();
            for (int i = 0; i < 10000; i++)
            {
                foreach (string newParent in parent)
                {
                    graph.AddDependency(newParent, i.ToString());
                }
            }

            foreach (string newParent in parent)
            {
                Assert.IsTrue(graph.HasDependents(newParent));
            }
        }

        [TestMethod]
        public void graphTest18()
        {
            DependencyGraph graph = new DependencyGraph();

            graph.AddDependency("a", "b");
            graph.AddDependency("c", "b");

            List<string> tempList = new List<string>();

            foreach (string parent in graph.GetDependees("b"))
            {
                tempList.Add(parent);
            }

            tempList.Add("d");

            Assert.IsTrue(new HashSet<string> { "a", "c", "d" }.SetEquals(tempList));
            Assert.IsTrue(new HashSet<string> { "a", "c" }.SetEquals(graph.GetDependees("b")));
        }

        [TestMethod]
        public void graphTest19()
        {
            DependencyGraph graph = new DependencyGraph();

            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");

            graph.RemoveDependency("z", "c");

            Assert.AreEqual(2, graph.Size);
        }
    }
}
