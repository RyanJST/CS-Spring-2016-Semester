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

        }
    }
}
