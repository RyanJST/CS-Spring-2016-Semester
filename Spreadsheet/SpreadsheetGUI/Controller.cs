using SS;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SpreadsheetGUI
{
    /// <summary>
    /// Class that controls the interactions between the Model(spreadsheet) and the view(Form).  Stores a window of a ISpreadsheet type,
    /// and a spreadsheet of AbstractSpreadsheet.
    /// </summary>
    public class Controller
    {
        /// <summary>
        /// Window of ISpreadsheet type.  Allows the controller to hook into the view through absract methods and set the UI to the correct responses.
        /// </summary>
        private ISpreadSheet window;

        /// <summary>
        /// Spreadsheet of AbstractSpreadsheet type.  Stores the data of the spreadsheet and allows manipulation of that data.
        /// </summary>
        private AbstractSpreadsheet sheet;

        /// <summary>
        /// Creates a controller to a window and a blank spreadsheet.  Uses a regex for the spreadsheet to ensure that the cell names do not go beyond
        /// A1-Z99.  Creates event handlers for the window that trigger methods in this class.
        /// </summary>
        /// <param name="window">window to be paired to the controller.</param>
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
            window.SaveClose += HandleSaveClose;

        }

        /// <summary>
        /// This version of the controller sets up a new window and controller that has the spreadsheet build from the old spreadsheet into the new one.
        /// Calls CreateSheet to populate the view with all the data of the new spreadsheet.
        /// </summary>
        /// <param name="window">Window that is paired with the controller</param>
        /// <param name="dest">String of the filepath of the saved file to open</param>
        public Controller(ISpreadSheet window, string dest)
        {
            this.window = window;
            try {
                using (TextReader reader = File.OpenText(dest))
                {
                    sheet = new Spreadsheet(reader);
                }
                window.Title = dest;
                window.FileChosenEvent += HandleFileChosen;
                window.CloseEvent += HandleClose;
                window.NewEvent += HandleNew;
                window.SaveEvent += HandleSave;
                window.ChangeContents += HandleChangeContents;
                window.ChangeSelection += HandleChangeSelection;
                window.SaveClose += HandleSaveClose;
                createSheet();
            }
            catch(Exception e)
            {
                window.Message = "Unable to open File." + e.Message;
            }
            
        }

        /// <summary>
        /// Method that happens when the user changes their selection.  Sets up the contents of the cellname box, cellValue box, and cellContents box, 
        /// to the correct result of the relevant cell.
        /// </summary>
        /// <param name="col">Number that correlates to the letter of the cellname</param>
        /// <param name="row">Number that correlates to the number of the cellname</param>
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

        /// <summary>
        /// This method is called when the ChangeContents event happens.  This event updates the result of the relevant cell in the spreadsheet
        /// and then updates every cells that depends on it in the GUI and in the spreadsheet.  
        /// </summary>
        /// <param name="obj">New cell contents to update the cell with</param>
        /// <param name="col">Number that correlates to the letter of the cellName</param>
        /// <param name="row">Number that correlates to the number of the cellName</param>
        private void HandleChangeContents(string obj, int col, int row)
        {
            char letter = (char)(97 + col);
            string cellName = letter.ToString().ToUpper() + (row + 1).ToString();
            
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
                window.cellContentsMainBox = sheet.GetCellContents(cellName).ToString(); 
            }
                window.cellValueMainBox = sheet.GetCellValue(cellName).ToString();
        }
            
        
        /// <summary>
        /// Method that triggers when the saveevent is triggered.  Saves the contents of the spreadsheet to a .ss file and sets the title of the window
        /// to the file path of the saved file.
        /// </summary>
        /// <param name="obj">string of the file path to save to</param>
        private void HandleSave(string obj)
        {
            if (!File.Exists(obj) || obj == window.Title || window.MessageYesNo("This file already exists, do you wish to overwrite?"))
            {
                try
                {

                    using (TextWriter write = File.CreateText(obj))
                    {
                        sheet.Save(write);
                    }
                    window.Title = obj;
                }

                catch (Exception e)
                {
                    window.Message = "Unable to save file \n" + e.Message;
                }
            }
        }

        /// <summary>
        /// Method that triggers when the new event is triggered.  Asks the window to open up a new window.
        /// </summary>
        private void HandleNew()
        {
            window.OpenNew();
        }

        /// <summary>
        /// Method that is triggered when the Close event is triggered.  Calls the window method that closes the current window.
        /// </summary>
        private void HandleClose()
        {
            window.DoClose();
        }

        /// <summary>
        /// Method that is trigged when the CloseSave event is triggered.  If the spreadsheet have been changed and not saved, it calls the window method
        /// that asks if the user wants to save.  Then it closes the window that is paired with the controller.  
        /// </summary>
        private void HandleSaveClose()
        {
            if (sheet.Changed)
            {
                if (window.MessageYesNo("You have unsaved Changes, do you wish to save?"))
                {
                    HandleSave(window.Title);
                }
            }
            //window.DoClose(); 
        }

        /// <summary>
        /// Method that happens when a file is chosen.  It calls the approriate method that opens up a new window with the data from the old spreadsheet.
        /// </summary>
        /// <param name="obj">string that has the filepath of the file to open.</param>
        private void HandleFileChosen(string obj)
        {
            window.OpenOldNew(obj);
                    
        }

        /// <summary>
        /// Method that is called by the constructor that takes the filepath of a file to open.
        /// Calls the view's updatetable method for every single cell that is not empty and update each one of them.
        /// </summary>
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
