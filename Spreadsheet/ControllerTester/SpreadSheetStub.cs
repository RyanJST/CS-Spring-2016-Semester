using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetGUI;

namespace ControllerTester
{
    class SpreadSheetStub : ISpreadSheet
    {
        private int updateCount = 0;

        private string cellValue;

        private string cellName;

        private string cellContents;
        public int UpdateCounter
        {
            get
            {
                return updateCount;
            }
        }


        public string getCellValue
        {
            get
            {
                return cellValue;
            }
        }

        public string getCellName
        {
            get
            {
                return cellName;
            }
        }

        public string getCellContents
        {
            get
            {
                return cellContents;
            }
        }
        public bool CalledDoClose
        {
            get; private set;
        }

        public bool CalledOpenNew
        {
            get; private set;
        }

        public bool CalledOpenOld
        {
            get; private set;
        }

        public bool CalledMessageYesNo
        {
            get; private set;
        }

        public void FireCloseEvent()
        {
            if (CloseEvent != null)
            {
                CloseEvent();
            }
        }

        public bool CalledUpdateTable
        {
            get; private set;
        }

        public void FireFileChosenEvent(string filename)
        {
            if (FileChosenEvent != null)
            {
                FileChosenEvent(filename);
            }
        }

        public void FireNewEvent()
        {
            if (NewEvent != null)
            {
                NewEvent();
            }
        }

        public void FireSaveEvent(string filePath)
        {
            if (SaveEvent != null)
            {
                SaveEvent(filePath);
            }

        }

        public void FireCloseSaveEvent()
        {
            if (SaveClose != null)
            {
                SaveClose();
            }

        }

        public void FireChangeSelection(int col, int row)
        {
            if(ChangeSelection != null)
            {
                ChangeSelection(col, row);
            }
        }

        public void FireChangeContents(string contents, int col, int row)
        {
            if(ChangeContents != null)
            {
                ChangeContents(contents, col, row);
            }
        }

        public string cellContentsMainBox
        {
            set
            {
                cellContents = value;    
            }
        }

        public string cellNameMainBox
        {
            set
            {
                cellName = value;
            }
        }

        public string cellValueMainBox
        {
            set
            {
                cellValue = value;
            }
            
        }

        public string Message
        {
            set
            {
                
            }
        }

        public string Title
        {
            get; set;
        }

        public event Action<string, int, int> ChangeContents;
        public event Action<int, int> ChangeSelection;
        public event Action CloseEvent;
        public event Action<string> FileChosenEvent;
        public event Action NewEvent;
        public event Action SaveClose;
        public event Action<string> SaveEvent;

        public void DoClose()
        {
            CalledDoClose = true;
        }

        public bool MessageYesNo(string value)
        {
            return true;
        }

        public void OpenNew()
        {
            CalledOpenNew = true;
        }



        public void OpenOldNew(string obj)
        {
            CalledOpenOld = true;  
        }

        public void updateTable(string obj, int row, int col)
        {
            CalledUpdateTable = true;
            updateCount++;
        }
    }
}
