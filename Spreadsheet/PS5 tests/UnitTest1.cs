//using System;
//using SS;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System.Collections.Generic;
//using Formulas;
//using Dependencies;

//namespace PS5_tests
//{
//    [TestClass]
//    public class UnitTest1
//    {
//        [TestMethod]
//        public void SpreadSheetConstruct1()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();
//        }

//        [TestMethod]
//        public void cellContents1()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1", "test");
//        }

//        [TestMethod]
//        public void cellContents2()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1", "test");

//            Assert.IsTrue(test.Contains("A1"));
//        }

//        [TestMethod]
//        public void cellContents3()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1", "test");
//            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
//            test = sheet.SetCellContents("A1", "test");
//            Assert.IsTrue(test.Contains("A1"));
//        }

//        [TestMethod]
//        public void cellContents4()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1", "test");
//            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
//            test = sheet.SetCellContents("A1", "test");

//            Assert.IsTrue(test.Contains("A3"));
//        }

//        [TestMethod]
//        public void cellContents5()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1", "test");
//            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
//            test = sheet.SetCellContents("D3", new Formula("D1 * 2"));
//            test = sheet.SetCellContents("A1", "test");

//            Assert.IsFalse(test.Contains("D3"));
//        }

//        [TestMethod]
//        public void cellContents6()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1", "test");
//            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("D1 * 2"));
//            test = sheet.SetCellContents("A1", "test");

//            Assert.IsFalse(test.Contains("A3"));
//        }

//        [TestMethod]
//        public void cellContents7()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1", "test");
//            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("D1 * 2"));

//            List<string> test2 = new List<string>();
//            foreach(string child in sheet.GetNamesOfAllNonemptyCells())
//            {
//                test2.Add(child);
//            }

//            Assert.IsFalse(test2.Contains("D1"));
//            Assert.IsTrue(test2.Contains("A2"));
//            Assert.IsTrue(test2.Contains("A3"));
//            Assert.IsTrue(test2.Contains("A1"));
//            Assert.AreEqual("", (string)(sheet.GetCellContents("D1")));
//        }

//        [TestMethod]
//        public void cellContents8()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1", "test");
//            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
//            test = sheet.SetCellContents("A3", 3.0);
//            test = sheet.SetCellContents("A1", "test");

//            Assert.IsFalse(test.Contains("A3"));
//        }

//        [TestMethod]
//        public void cellContents9()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1", "test");
//            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
//            test = sheet.SetCellContents("A3", 3.0);
//            test = sheet.SetCellContents("A1", "test");

//            Assert.AreEqual(3.0, sheet.GetCellContents("A3"));
//            Assert.AreEqual("test", sheet.GetCellContents("A1"));
//            Assert.AreEqual("", sheet.GetCellContents("D1"));
//        }

//        [TestMethod]
//        [ExpectedException(typeof(InvalidNameException))]
//        public void cellContents10()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A0", "test");
//            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
//            test = sheet.SetCellContents("A3", 3.0);
//            test = sheet.SetCellContents("A1", "test");
//        }

//        [TestMethod]
//        [ExpectedException(typeof(InvalidNameException))]
//        public void cellContents11()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("AA", "test");
//            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
//            test = sheet.SetCellContents("A3", 3.0);
//            test = sheet.SetCellContents("A1", "test");
//        }

//        [TestMethod]
//        [ExpectedException(typeof(InvalidNameException))]
//        public void cellContents12()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1x", "test");
//            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
//            test = sheet.SetCellContents("A3", 3.0);
//            test = sheet.SetCellContents("A1", "test");
//        }

//        [TestMethod]
//        public void cellContents13()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1691691631957", "test");
//            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
//            test = sheet.SetCellContents("A3", 3.0);
//            test = sheet.SetCellContents("A1", "test");
//        }

//        [TestMethod]
//        public void cellContents14()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1691691631957", "test");
//            test = sheet.SetCellContents("A2", new Formula("A1 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("A2 * 2"));
//            test = sheet.SetCellContents("A3", 3.0);
//            test = sheet.SetCellContents("A1", new Formula("A1691691631957"));

//            Assert.IsTrue(test.Contains("A2"));
//        }

//        [TestMethod]
//        [ExpectedException(typeof(CircularException))]
//        public void cellContents15()
//        {
//            AbstractSpreadsheet sheet = new Spreadsheet();

//            ISet<string> test = sheet.SetCellContents("A1", "test");
//            test = sheet.SetCellContents("A2", new Formula("A3 * 2"));
//            test = sheet.SetCellContents("A3", new Formula("A4 * 2"));
//            test = sheet.SetCellContents("A4", new Formula("A2 * 2"));
//            test = sheet.SetCellContents("A3", 3.0);
//            test = sheet.SetCellContents("A1", "test");
//        }
//    }
//}
