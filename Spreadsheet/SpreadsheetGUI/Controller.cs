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
            sheet = new Spreadsheet(new System.Text.RegularExpressions.Regex(@"^[A-Z]+[1-9]{1}[0-9]{0,1}$"));
            window.Title = "Spreadsheet";
            window.FileChosenEvent += HandleFileChosen;
            window.CloseEvent += HandleClose;
            window.NewEvent += HandleNew;
            window.SaveEvent += HandleSave;
            window.ChangeContents += HandleChangeContents;
            window.ChangeSelection += HandleChangeSelection;
            
        }


        public Controller(ISpreadSheet window, string dest)
        {
            this.window = window;
            try {
                using (TextReader reader = File.OpenText(dest))
                {
                    sheet = new Spreadsheet(reader);
                }
            }
            catch(Exception e)
            {
                window.Message = "Unable to open File." + e.Message;
            }
            window.Title = dest;
            window.FileChosenEvent += HandleFileChosen;
            window.CloseEvent += HandleClose;
            window.NewEvent += HandleNew;
            window.SaveEvent += HandleSave;
            window.ChangeContents += HandleChangeContents;
            window.ChangeSelection += HandleChangeSelection;
            createSheet();
        }
        private void HandleChangeSelection(int col, int row)
        {
            char letter = (char)(97 + col);
            string cellName = letter.ToString().ToUpper() + (row + 1).ToString();
            
            window.cellNameMainBox = cellName;

            window.cellValueMainBox = sheet.GetCellValue(cellName).ToString();

            if (!(sheet.GetCellContents(cellName) is string) && !(sheet.GetCellContents(cellName) is double) && !(sheet.GetCellContents(cellName) is FormulaError))
            {
                window.cellContentsMainBox = "= " + sheet.GetCellContents(cellName).ToString();
            }
            else
            {
                window.cellContentsMainBox = sheet.GetCellContents(cellName).ToString();
            }

        }

        private void HandleChangeContents(string obj, int col, int row)
        {
            char letter = (char)(97 + col);
            string cellName = letter.ToString().ToUpper() + (row + 1).ToString();
            string cellName2 = letter.ToString().ToUpper() + (row).ToString();
            try {
                foreach (string cell in sheet.SetContentsOfCell(cellName, obj))
                {
                    char letter2 = cell[0];
                    int letNum = letter2 - 65;
                    int numRow;
                    if (int.TryParse(cell.Substring(1), out numRow))
                    {
                        window.updateTable(sheet.GetCellValue(cell).ToString(), letNum, numRow - 1);
                    }

                }
            }
            catch(Exception e)
            {
                window.Message = "Unable to do change " + e.Message; 
            }
            if (!(sheet.GetCellContents(cellName) is string) && char.IsLetter(sheet.GetCellContents(cellName).ToString()[0]))
            {
                window.cellValueMainBox = "= " + sheet.GetCellValue(cellName).ToString();
            }
            else
            {
                window.cellValueMainBox = sheet.GetCellValue(cellName).ToString();
            }
        }
            
        

        private void HandleSave(string obj)
        {
            try {
                using(TextWriter write = File.CreateText(obj)){
                    sheet.Save(write);
                }
                window.Title = obj;
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
                window.MessageYesNo = window.Title;
            }
            window.DoClose();
        }

        private void HandleFileChosen(string obj)
        {
            window.OpenOldNew(obj);
                    
        }

        private void createSheet()
        {
            try {
                foreach (string cellName in sheet.GetNamesOfAllNonemptyCells())
                {
                    char letter = cellName[0];
                    int letNum = letter - 65;
                    int numRow;
                    if (int.TryParse(cellName.Substring(1), out numRow))
                    {

                        window.updateTable(sheet.GetCellValue(cellName).ToString(), letNum, numRow - 1);

                    }
                } 

            }
            catch(Exception e)
            {
                window.Message = "Unable to open file\n" + e.Message;
            }
}
    }
}
