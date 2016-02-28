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
            window.FileChosenEvent += HandleFileChosen;
            window.CloseEvent += HandleClose;
            window.NewEvent += HandleNew;
            window.SaveEvent += HandleSave;

        }

        private void HandleSave(string obj)
        {
            throw new NotImplementedException();
        }

        private void HandleNew()
        {
            sheet = new Spreadsheet();
        }

        private void HandleClose()
        {
            window.DoClose();
        }

        private void HandleFileChosen(string obj)
        {
            try
            {
                TextReader reader = File.OpenText(obj);
                sheet = new Spreadsheet(reader);
                window.Title = obj;
            }
            catch(Exception e)
            {
                window.Message = "Unable to open file\n" + e.Message;
            }
        }
    }
}
