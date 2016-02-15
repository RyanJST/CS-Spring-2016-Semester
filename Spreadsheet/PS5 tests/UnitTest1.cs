using System;
using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Formulas;
using Dependencies;

namespace PS5_tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SpreadSheetConstruct1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
        }

        [TestMethod]
        public void cellContents1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            ISet<string> test = sheet.SetCellContents("A1", "test");
        }

        [TestMethod]
        public void cellContents2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            ISet<string> test = sheet.SetCellContents("A1", "test");

            Assert.IsTrue(test.Contains("A1"));
        }
    }
}
