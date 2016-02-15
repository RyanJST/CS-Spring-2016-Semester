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

        [TestMethod]
        public void cellContents3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            ISet<string> test = sheet.SetCellContents("A1", "test");
            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
            test = sheet.SetCellContents("A1", "test");
            Assert.IsTrue(test.Contains("A1"));
        }

        [TestMethod]
        public void cellContents4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            ISet<string> test = sheet.SetCellContents("A1", "test");
            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
            test = sheet.SetCellContents("A1", "test");

            Assert.IsTrue(test.Contains("A3"));
        }

        [TestMethod]
        public void cellContents5()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();

            ISet<string> test = sheet.SetCellContents("A1", "test");
            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
            test = sheet.SetCellContents("D3", new Formula("D1 * 2"));
            test = sheet.SetCellContents("A1", "test");

            Assert.IsFalse(test.Contains("D3"));
        }
    }
}
