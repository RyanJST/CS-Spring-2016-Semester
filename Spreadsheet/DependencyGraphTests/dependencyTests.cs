using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;

namespace DependencyGraphTests
{
    /// <summary>
    /// This is the test class for the DependencyGraph.  Each test method strives to test
    ///the graph 100% and also run speed tests.  
    /// </summary>
    [TestClass]
    public class dependencyTests
    {
        /// <summary>
        /// Tests a normal construction, nothing else done, no relationships added, just test the constructor
        /// </summary>
        [TestMethod]
        public void graphTest1()
        {
            DependencyGraph graph = new DependencyGraph();
        }

        /// <summary>
        /// Checks to see if the null catcher works in hasDependees
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void graphTest2()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.HasDependees(null);
        }

        /// <summary>
        /// Checks to see that size of the graph is zero off the bat
        /// </summary>
        [TestMethod]
        public void graphTest3()
        {
            DependencyGraph graph = new DependencyGraph();
            Assert.AreEqual(graph.Size, 0);
        }


        /// <summary>
        /// First tests adding ten parents that can have up to 100 children.
        /// Then checks to make sure that each dependee has at least one child
        /// </summary>
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

        /// <summary>
        /// Creates over a thousand parents, and gives each ten children
        /// then checks on whether each child has a parent or not
        /// </summary>
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

        /// <summary>
        /// creates 100 relationships in the graph, and then removes a random relationship
        /// Then tests on whether or not the relationship is still in the graph or not
        /// </summary>

        [TestMethod]
        public void graphTest6()
        {
            DependencyGraph graph = new DependencyGraph();

            Random rand = new Random();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] parent = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            
            foreach(string parents in parent)
            {
                foreach(string child in children)
                {
                    graph.AddDependency(parents, child);
                }
            }

            int ranNum1 = rand.Next(0, 10);

            int ranNum2 = rand.Next(0, 10);

            graph.RemoveDependency(parent[ranNum1], children[ranNum2]);



            foreach(string aChildren in graph.GetDependents(parent[ranNum1]))
            {
                Assert.AreNotEqual(children[ranNum2], aChildren);
            }
        }

        /// <summary>
        /// creates one hundred relationships, annd sets all of the 
        /// </summary>
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

            Random rand = new Random();

            int ranNum = rand.Next(0, 10);

            foreach (string aChildren in graph.GetDependents(parent[ranNum]))
            {
                testChildren[i] = aChildren;
                i++;
            }
            for (int j = 0; j < children.Length; j++){
                Assert.AreEqual(children[j], testChildren[j]);
            }
        }

        /// <summary>
        /// creates one hundred relationships and gets the dependees of a random dependent.
        /// Afterwards, puts all the parents into a test array, and confirms that the two arrays
        /// (parent and testParent) are the same
        /// </summary>
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

            Random rand = new Random();

            int ranNum = rand.Next(0, 10);

            foreach (string aParents in graph.GetDependees(children[ranNum]))
            {
                testChildren[i] = aParents;
                i++;
            }
            for (int j = 0; j < parent.Length; j++)
            {
                Assert.AreEqual(parent[j], testChildren[j]);
            }
        }

        /// <summary>
        /// Creates a random amount of relationships(up to 100) and tests the size, to make sure that
        /// the size is equal to the amount of relationships
        /// </summary>
        [TestMethod]
        public void graphTest9()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] parent = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };

            Random rand = new Random();

            int ranNum1 = rand.Next(0, parent.Length);
            int ranNum2 = rand.Next(0, children.Length);
            for(int i = 0; i < ranNum1; i++)
            {
                for(int j = 0; j < ranNum2; j++)
                {
                    graph.AddDependency(parent[i], children[j]);
                }
            }
            Assert.AreEqual(ranNum1 * ranNum2, graph.Size);
        }


        /// <summary>
        /// Creates one hundred relationships and replaces the dependents of one of the dependees with
        /// a new set.  Then tests to make sure that the new dependents are them.
        /// </summary>
        [TestMethod]
        public void graphTest10()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] newChildren = new string[10] {"k", "l", "m", "n", "o", "p", "q", "r", "s", "t"};
            string[] parent = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };

            string[] testChild = new string[10];

            int i = 0;

            Random rand = new Random();

            int ranNum = rand.Next(0, parent.Length);

            foreach (string parents in parent)
            {
                foreach (string child in children)
                {
                    graph.AddDependency(parents, child);
                }
            }

            graph.ReplaceDependents(parent[ranNum], newChildren);

            foreach(string child in graph.GetDependents(parent[ranNum]))
            {
                testChild[i] = child;
                i++;
            }

            for(int j = 0; j < newChildren.Length; j++)
            {
                Assert.AreEqual(newChildren[j], testChild[j]);
            }
        }

        /// <summary>
        /// Adds and removes the same dependency 100,000 times, to test the speed of the execution
        /// and ensure that it is instantaneous to the end user.
        /// </summary>
        [TestMethod]
        public void graphTest11()
        {
            DependencyGraph graph = new DependencyGraph();
            
            for(int i = 0; i< 100000; i++)
            {
                graph.AddDependency("a", "b");
                graph.RemoveDependency("a", "b");
            }
        }

        /// <summary>
        /// creates one hundred relationshipsand replaces the dependees of a certain value
        /// Then tests to ensure that the dependees are the new dependees 
        /// </summary>
        [TestMethod]
        public void graphTest12()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] newParents = new string[10] { "k", "l", "m", "n", "o", "p", "q", "r", "s", "t" };
            string[] parent = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };

            string[] testChild = new string[10];

            int i = 0;

            Random rand = new Random();

            int ranNum = rand.Next(0, children.Length);

            foreach (string parents in parent)
            {
                foreach (string child in children)
                {
                    graph.AddDependency(parents, child);
                }
            }

            graph.ReplaceDependees(children[ranNum], newParents);

            foreach (string newParent in graph.GetDependees(children[ranNum]))
            {
                testChild[i] = newParent;
                i++;
            }

            for (int j = 0; j < newParents.Length; j++)
            {
                Assert.AreEqual(newParents[j], testChild[j]);
            }
        }

        /// <summary>
        /// Creates one hundred relationships and randomly removes the dependencies of a certain 
        /// dependee and tests to make sure that there are no dependencies that involve that dependee.
        /// </summary>
        [TestMethod]
        public void graphTest13()
        {
            DependencyGraph graph = new DependencyGraph();

            string[] children = new string[10] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j" };
            string[] parent = new string[10] { "k", "l", "m", "n", "o", "p", "q", "r", "s", "t" };

            Random rand = new Random();

            int ranNum = rand.Next(0, parent.Length);

            foreach(string child in children)
            {
                foreach(string newParent in parent)
                {
                    graph.AddDependency(newParent, child);
                }
            }

            foreach (string child in children)
            {
                graph.RemoveDependency(parent[ranNum], child);
            }

            Assert.IsFalse(graph.HasDependees(parent[ranNum]));
        }

        /// <summary>
        /// This method attempts to create one hundred relationship, but since it uses the same dependent(a)
        /// it does not add any beyond the first attempt for each dependee.  Then it tests that there are only
        /// 10 relationship(one per dependee)
        /// </summary>
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

        /// <summary>
        /// Ensures that the return results from the getDependents method is not passing a reference, but the value instead
        /// to make sure that the base results cannot be altered outside of the class methods.
        /// </summary>
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

        /// <summary>
        /// This tests creating over 10,000 dependees, each with 10 dependents, primarily ensuring speed
        /// It also tests speed of looking up the dependees from the dependents.
        /// </summary>
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

        /// <summary>
        /// Tests creating 10 dependees, each with 10000 dependents, for speed
        /// and checks to see how fast it would take for finding the dependents of a dependee
        /// </summary>
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

        /// <summary>
        /// Checks to make sure that the getDependees method does not return a reference,
        /// but instead the value itself.  
        /// </summary>
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

        /// <summary>
        /// checks on making sure that remove dependency does not crash when the value inputed
        /// does not exist in the graph.
        /// </summary>
        [TestMethod]
        public void graphTest19()
        {
            DependencyGraph graph = new DependencyGraph();

            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");

            graph.RemoveDependency("z", "c");

            Assert.AreEqual(2, graph.Size);
        }

        /// <summary>
        /// Tests whether the null catcher works for hasDependents
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void graphTest20()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.HasDependents(null);
        }
    }
}
