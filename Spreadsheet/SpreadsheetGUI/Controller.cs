using SS;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SpreadsheetGUI
{
    public class Controller
    {

        private ISpreadSheet window;

        private AbstractSpreadsheet sheet;

        public Controller(ISpreadSheet window)
        {
            this.window = window;
            sheet = new Spreadsheet();
            window.FileChosenEvent += HandleFileChosen;
            window.CloseEvent += HandleClose;
            window.NewEvent += HandleNew;
            window.SaveEvent += HandleSave;
            window.ChangeContents += HandleChangeContents;
            window.ChangeSelection += HandleChangeSelection;
        }

        private void HandleChangeSelection(int col, int row)
        {
            char letter = (char)(97 + col);
            string cellName = letter.ToString().ToUpper() + (row + 1).ToString();
            string cellName2 = letter.ToString().ToUpper() + (row).ToString();
            window.cellNameMainBox = cellName2;

            window.cellValueMainBox = sheet.GetCellValue(cellName).ToString();

            window.cellContentsMainBox = sheet.GetCellContents(cellName).ToString();
            
        }

        private void HandleChangeContents(string obj, int col, int row)
        {
            char letter = (char)(97 + col);
            string cellName = letter.ToString().ToUpper() + (row + 1).ToString();
            string cellName2 = letter.ToString().ToUpper() + (row).ToString();

            sheet.SetContentsOfCell(cellName, obj);
            window.updateTable(sheet.GetCellValue(cellName).ToString(), col, row);
            
        }

        private void HandleSave(string obj)
        {
            try {
                TextWriter write = File.CreateText(obj);
                sheet.Save(write);
            }

            catch(Exception e)
            {
                window.Message = "Unable to save file \n" + e.Message;
            }
        }

        private void HandleNew()
        {
            window.OpenNew();
        }

        private void HandleClose()
        {
            if (sheet.Changed)
            {
                
            }
            window.DoClose();
        }

        private void HandleFileChosen(string obj)
        {
            try
            {
                TextReader reader = File.OpenText(obj);
                sheet = new Spreadsheet(reader);
                foreach(string cellName in sheet.GetNamesOfAllNonemptyCells())
                {
                    char letter = cellName[0];
                    int letNum = letter - 97;
                    int numRow;
                    if(int.TryParse(cellName.Substring(1), out numRow))
                    {
                        window.updateTable(sheet.GetCellValue(cellName).ToString(), letNum, numRow - 1);
                    }
                }
                window.Title = obj;
              
            }
            catch(Exception e)
            {
                window.Message = "Unable to open file\n" + e.Message;
            }
        }
    }
}
