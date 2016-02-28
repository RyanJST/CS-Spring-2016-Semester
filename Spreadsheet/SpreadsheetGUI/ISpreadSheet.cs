using System;

namespace SpreadsheetGUI
{
    public interface ISpreadSheet
    {
        event Action<string> FileChosenEvent;

        event Action CloseEvent;

        event Action NewEvent;

        event Action<string> SaveEvent;

        string Title { set; }

        void DoClose();

        void OpenNew();

        string Message { set; }

        string cellNameMainBox { set; }

        string cellValueMainBox { set; }

        string cellContentsMainBox { set; }


    }
}