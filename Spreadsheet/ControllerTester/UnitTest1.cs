using System;
using SpreadsheetGUI;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ControllerTester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            SpreadSheetStub stub = new SpreadSheetStub();
            Controller control = new Controller(stub);

            stub.FireNewEvent();
            stub.FireCloseEvent();

            Assert.IsTrue(stub.CalledDoClose);
            Assert.IsTrue(stub.CalledOpenNew);
        }

        [TestMethod]
        public void TestMethod2()
        {
            SpreadSheetStub stub = new SpreadSheetStub();
            Controller control = new Controller(stub);

            stub.FireFileChosenEvent("C:\\Users\\Lora E.Cooper\\Documents\\Test2.ss");

            Assert.IsTrue(stub.CalledOpenOld);
        }

        [TestMethod]
        public void TestMethod3()
        {
            SpreadSheetStub stub = new SpreadSheetStub();
            Controller control = new Controller(stub);

            stub.FireChangeContents("Ton", 0, 0);

            Assert.AreEqual("Ton", stub.getCellValue);
        }

        [TestMethod]
        public void TestMethod4()
        {
            SpreadSheetStub stub = new SpreadSheetStub();
            Controller control = new Controller(stub, "C:\\Users\\Lora E. Cooper\\Desktop\\Test2.ss");

            Assert.AreEqual(3, stub.UpdateCounter);
        }

        [TestMethod]
        public void TestMethod5()
        {
            SpreadSheetStub stub = new SpreadSheetStub();
            Controller control = new Controller(stub);

            stub.FireChangeSelection(0, 0);

            Assert.AreEqual("A1", stub.getCellName);
            Assert.AreEqual("", stub.getCellValue);
            Assert.AreEqual("", stub.getCellContents);

            stub.FireChangeContents("54", 0, 0);

            Assert.AreEqual("A1", stub.getCellName);
            Assert.AreEqual("54", stub.getCellValue);
            

        }

        [TestMethod]
        public void TestMethod6()
        {
            SpreadSheetStub stub = new SpreadSheetStub();
            Controller control = new Controller(stub);

            stub.FireChangeSelection(0, 0);

            stub.FireChangeContents("54", 0, 0);

            stub.FireCloseSaveEvent();

        }

        [TestMethod]
        public void TestMethod7()
        {
            SpreadSheetStub stub = new SpreadSheetStub();
            Controller control = new Controller(stub);

            stub.FireChangeSelection(0, 0);

            stub.FireChangeContents("54", 0, 0);

            stub.FireSaveEvent("../../SpreadsheetTest.ss");

            stub.FireFileChosenEvent("../../SpreadsheetTest.ss");

            Assert.IsTrue(stub.CalledOpenOld);
        }

        [TestMethod]
        public void TestMethod8()
        {
            SpreadSheetStub stub = new SpreadSheetStub();
            Controller control = new Controller(stub);

            stub.FireChangeSelection(0, 0);

            Assert.AreEqual("A1", stub.getCellName);
            Assert.AreEqual("", stub.getCellValue);
            Assert.AreEqual("", stub.getCellContents);

            stub.FireChangeContents("= 54 * 10", 0, 0);

            Assert.AreEqual("A1", stub.getCellName);
            Assert.AreEqual("540", stub.getCellValue);

            stub.FireChangeSelection(0, 0);

            Assert.AreEqual("= 54 * 10 ", stub.getCellContents);
        }

        [TestMethod]
        public void TestMethod9()
        {
            SpreadSheetStub stub = new SpreadSheetStub();
            Controller control = new Controller(stub, "D:\\NonExistantDive\\Test2.ss");

            Assert.IsTrue(stub.CalledMessage);
        }

        [TestMethod]
        public void TestMethod10()
         {
            SpreadSheetStub stub = new SpreadSheetStub();
            Controller control = new Controller(stub);

            stub.FireChangeSelection(0, 0);

            Assert.AreEqual("A1", stub.getCellName);
            Assert.AreEqual("", stub.getCellValue);
            Assert.AreEqual("", stub.getCellContents);

            stub.FireChangeContents("= 54 * 10", 0, 0);

            Assert.AreEqual("A1", stub.getCellName);
            Assert.AreEqual("540", stub.getCellValue);

            stub.FireChangeSelection(0, 0);

            Assert.AreEqual("= 54 * 10 ", stub.getCellContents);

            stub.FireChangeContents("= A1", 0, 0);

            Assert.IsTrue(stub.CalledMessage);

            stub.FireChangeSelection(0, 0);

            Assert.AreEqual("= 54 * 10 ", stub.getCellContents);
        }

        [TestMethod]
        public void TestMethod11()
        {
            SpreadSheetStub stub = new SpreadSheetStub();
            Controller control = new Controller(stub);

            stub.FireSaveEvent("D:\\NonExistantDrive\\Test10.ss");

            Assert.IsTrue(stub.CalledMessage);
            
        }
    }
}
